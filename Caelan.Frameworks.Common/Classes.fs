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
    override __.AddMappingConfigurations(mappingExpression) = 
        AutoMapperExtender.IgnoreAllNonExisting(mappingExpression) |> ignore
    abstract AfterBuild : 'TSource * 'TDestination ref -> unit
    override __.AfterBuild(_, _) = ()
    
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
        (destination := match source = null || source.Equals(Unchecked.defaultof<'TSource>) with
                        | true -> Unchecked.defaultof<'TDestination>
                        | _ -> Mapper.DynamicMap<'TSource, 'TDestination>(source))
        |> ignore
    
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination) = 
        async { return this.Build(source, ref destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = async { return this.BuildList(sourceList) } |> Async.StartAsTask

[<AbstractClass>]
[<Sealed>]
type GenericBuilder() = 
    
    static member private FindCustomBuilderType (builderType : Type) (assembly : Assembly) = 
        match assembly.GetTypes() |> Seq.tryFind (fun t -> t.BaseType = builderType) with
        | Some(value) -> value
        | None -> 
            query { 
                for refAssembly in (assembly.GetReferencedAssemblies()
                                    |> Seq.sortBy (fun t -> t.Name)
                                    |> Seq.map (fun t -> Assembly.Load(t))
                                    |> Seq.collect 
                                           (fun t -> t.GetTypes() |> Seq.filter (fun x -> x.BaseType = builderType))) do
                    select refAssembly
                    exactlyOneOrDefault
            }
    
    static member CreateGenericBuilder<'TBuilder, 'TSource, 'TDestination when 'TBuilder :> BaseBuilder<'TSource, 'TDestination>>() = 
        let builderType = typedefof<'TBuilder>
        
        let builder = 
            ((match builderType.IsGenericTypeDefinition with
              | true -> builderType.MakeGenericType(typedefof<'TSource>, typedefof<'TDestination>)
              | _ -> builderType)
             |> Activator.CreateInstance) :?> 'TBuilder
        
        let customBuilder = 
            match Assembly.GetAssembly(typedefof<'TDestination>) 
                  |> GenericBuilder.FindCustomBuilderType(builder.GetType()) with
            | null -> 
                (match Assembly.GetEntryAssembly() with
                 | null -> Assembly.GetCallingAssembly()
                 | _ -> Assembly.GetEntryAssembly())
                |> GenericBuilder.FindCustomBuilderType(builder.GetType())
            | customBuilderType -> customBuilderType
        
        match customBuilder with
        | null -> builder
        | _ -> Activator.CreateInstance(customBuilder) :?> 'TBuilder
    
    static member Create<'TSource, 'TDestination when 'TSource : equality and 'TDestination : equality and 'TSource : null and 'TDestination : null>() = 
        GenericBuilder.CreateGenericBuilder<BaseBuilder<'TSource, 'TDestination>, 'TSource, 'TDestination>()

[<Sealed>]
[<AbstractClass>]
type BuilderConfiguration() = 
    static member Configure() = 
        Mapper.Initialize(fun a -> 
            Assembly.GetCallingAssembly().GetTypes()
            |> (Seq.append (Assembly.GetCallingAssembly().GetReferencedAssemblies()
                            |> Seq.map (fun t -> Assembly.Load(t))
                            |> Seq.collect (fun t -> t.GetTypes())))
            |> Seq.filter 
                   (fun t -> 
                   (typeof<Profile>).IsAssignableFrom(t) = true && t.GetConstructor(Type.EmptyTypes) <> null 
                   && t.IsGenericTypeDefinition = false)
            |> Seq.map (fun t -> Activator.CreateInstance(t) :?> Profile)
            |> Seq.iter (fun t -> a.AddProfile(t)))
