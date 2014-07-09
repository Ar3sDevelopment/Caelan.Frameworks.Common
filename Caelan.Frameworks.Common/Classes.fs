namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open System.Threading.Tasks
open System.Linq
open AutoMapper
open AutoMapper.Internal
open Caelan.Frameworks.Common.Extenders

type BaseBuilder<'TSource, 'TDestination when 'TSource : equality and 'TDestination : equality and 'TSource : null and 'TDestination : null>() = 
    inherit Profile()
    abstract AddMappingConfigurations : IMappingExpression<'TSource, 'TDestination> -> unit
    override this.AddMappingConfigurations(mappingExpression : IMappingExpression<'TSource, 'TDestination>) = 
        AutoMapperExtender.IgnoreAllNonExisting(mappingExpression) |> ignore
    abstract AfterBuild : 'TSource * 'TDestination ref -> unit
    override this.AfterBuild(source : 'TSource, destination : 'TDestination ref) = ()
    
    override this.Configure() = 
        base.Configure()
        let mappingExpression = Mapper.CreateMap<'TSource, 'TDestination>()
        mappingExpression.AfterMap(fun source destination -> 
            let refDest = ref destination
            this.AfterBuild(source, refDest) |> ignore)
        |> ignore
        this.AddMappingConfigurations(mappingExpression)
    
    member this.Build(source : 'TSource) = 
        match source with
        | null -> Unchecked.defaultof<'TDestination>
        | _ -> 
            let dest = ref Unchecked.defaultof<'TDestination>
            if (!dest = null) then dest := Activator.CreateInstance<'TDestination>()
            this.Build(source, dest)
            !dest
    
    member this.BuildList(sourceList) = sourceList |> Seq.map (fun source -> this.Build(source))
    
    member this.Build(source : 'TSource, destination : 'TDestination ref) = 
        destination := match source = null || source.Equals(Unchecked.defaultof<'TSource>) with
                       | true -> Unchecked.defaultof<'TDestination>
                       | _ -> Mapper.DynamicMap<'TSource, 'TDestination>(source)
        ()
    
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination) = 
        async { return this.Build(source, ref destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = async { return this.BuildList(sourceList) } |> Async.StartAsTask

[<AbstractClass>]
[<Sealed>]
type GenericBuilder() = 
    
    static member CreateGenericBuilder<'TBuilder, 'TSource, 'TDestination when 'TBuilder :> BaseBuilder<'TSource, 'TDestination>>() = 
        let builderType = typedefof<'TBuilder>
        
        let builder = 
            (match builderType.IsGenericType with
             | true -> 
                 Activator.CreateInstance(builderType.MakeGenericType(typedefof<'TSource>, typedefof<'TDestination>))
             | _ -> Activator.CreateInstance(builderType)) :?> 'TBuilder
        
        let mutable assembly = Assembly.GetAssembly(typedefof<'TDestination>)
        
        let mutable customBuilder = 
            match assembly.GetTypes() |> Array.tryFind (fun t -> t.BaseType = builder.GetType()) with
            | None -> null
            | Some(value) -> value
        if customBuilder = null then 
            let referAssemblies = assembly.GetReferencedAssemblies() |> Array.sortBy (fun t -> t.Name)
            customBuilder <- referAssemblies.Select(fun t -> Assembly.Load(t))
                                            .SelectMany(fun t -> 
                                            t.GetTypes().Where(fun x -> x.BaseType = builder.GetType()))
                                            .SingleOrDefault()
        if customBuilder = null then 
            assembly <- match Assembly.GetEntryAssembly() with
                        | null -> Assembly.GetCallingAssembly()
                        | _ -> Assembly.GetEntryAssembly()
            customBuilder <- match assembly.GetTypes() |> Array.tryFind (fun t -> t.BaseType = builder.GetType()) with
                             | None -> null
                             | Some(value) -> value
            if customBuilder = null then 
                let referAssemblies = assembly.GetReferencedAssemblies() |> Array.sortBy (fun t -> t.Name)
                customBuilder <- referAssemblies.Select(fun t -> Assembly.Load(t))
                                                .SelectMany(fun t -> 
                                                t.GetTypes().Where(fun x -> x.BaseType = builder.GetType()))
                                                .SingleOrDefault()
                if customBuilder = null then 
                    if Mapper.FindTypeMapFor<'TSource, 'TDestination>() = null then Mapper.AddProfile(builder)
                    builder
                else Activator.CreateInstance(customBuilder) :?> 'TBuilder
            else Activator.CreateInstance(customBuilder) :?> 'TBuilder
        else Activator.CreateInstance(customBuilder) :?> 'TBuilder
    
    static member Create<'TSource, 'TDestination when 'TSource : equality and 'TDestination : equality and 'TSource : null and 'TDestination : null>() = 
        GenericBuilder.CreateGenericBuilder<BaseBuilder<'TSource, 'TDestination>, 'TSource, 'TDestination>()

[<Sealed>]
[<AbstractClass>]
type BuilderConfiguration() = 
    static member Configure() = 
        let profiles = 
            Assembly.GetCallingAssembly().GetTypes()
            |> (Seq.append (Assembly.GetCallingAssembly().GetReferencedAssemblies()
                |> Seq.map (fun t -> Assembly.Load(t))
                    |> Seq.collect (fun t -> t.GetTypes())))
            |> Seq.filter 
                   (fun t -> 
                   (typeof<Profile>).IsAssignableFrom(t) = true && t.GetConstructor(Type.EmptyTypes) <> null 
                   && t.IsGenericType = false)
            |> Seq.map (fun t -> Activator.CreateInstance(t) :?> Profile)
            |> List.ofSeq
        Mapper.Initialize(fun a -> profiles |> List.iter (fun t -> a.AddProfile(t)))
