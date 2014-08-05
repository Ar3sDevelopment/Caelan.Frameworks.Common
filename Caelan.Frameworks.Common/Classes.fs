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
    override __.AddMappingConfigurations(mappingExpression : IMappingExpression<'TSource, 'TDestination>) = 
        AutoMapperExtender.IgnoreAllNonExisting(mappingExpression) |> ignore
    abstract AfterBuild : 'TSource * 'TDestination ref -> unit
    override __.AfterBuild(_ : 'TSource, _ : 'TDestination ref) = ()
    
    override this.Configure() = 
        base.Configure()
        let mappingExpression = Mapper.CreateMap<'TSource, 'TDestination>()
        mappingExpression.AfterMap(fun source destination -> 
            let refDest = ref destination
            this.AfterBuild(source, refDest))
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
    
    member __.Build(source : 'TSource, destination : 'TDestination ref) = 
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
    
    static member private FindCustomBuilderType (builderType : Type) (assembly : Assembly) = 
        match assembly.GetTypes() |> Array.tryFind (fun t -> t.BaseType = builderType) with
        | Some(value) -> value
        | None -> 
            query { 
                for refAssembly in (assembly.GetReferencedAssemblies()
                                    |> Array.sortBy (fun t -> t.Name)
                                    |> Array.map (fun t -> Assembly.Load(t))
                                    |> Array.collect 
                                           (fun t -> t.GetTypes() |> Array.filter (fun x -> x.BaseType = builderType))) do
                    select refAssembly
                    exactlyOneOrDefault
            }
    
    static member CreateGenericBuilder<'TBuilder, 'TSource, 'TDestination when 'TBuilder :> BaseBuilder<'TSource, 'TDestination>>() = 
        let builderType = typedefof<'TBuilder>
        
        let builder = 
            (match builderType.IsGenericTypeDefinition with
             | true -> 
                 builderType.MakeGenericType(typedefof<'TSource>, typedefof<'TDestination>) |> Activator.CreateInstance
             | _ -> Activator.CreateInstance(builderType)) :?> 'TBuilder
        
        let assembly = Assembly.GetAssembly(typedefof<'TDestination>)
        
        let customBuilder = 
            match assembly |> GenericBuilder.FindCustomBuilderType(builder.GetType()) with
            | null -> 
                (match Assembly.GetEntryAssembly() with
                 | null -> Assembly.GetCallingAssembly()
                 | _ -> Assembly.GetEntryAssembly())
                |> GenericBuilder.FindCustomBuilderType(builder.GetType())
            | customBuilderType -> customBuilderType
        if customBuilder = null then builder
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
                   && t.IsGenericTypeDefinition = false)
            |> Seq.map (fun t -> Activator.CreateInstance(t) :?> Profile)
            |> Array.ofSeq
        Mapper.Initialize(fun a -> profiles |> Array.iter (fun t -> a.AddProfile(t)))
