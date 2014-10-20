namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open Caelan.Frameworks.Common.Interfaces

type BaseBuilder<'TSource, 'TDestination when 'TSource : equality and 'TDestination : equality and 'TSource : null and 'TDestination : null>(mapper : IMapper<'TSource, 'TDestination>) = 
    abstract AfterBuild : 'TSource * 'TDestination ref -> unit
    override __.AfterBuild(_, _) = ()
    
    member this.Build(source : 'TSource) = 
        mapper.Map(source)
    
    member this.BuildList(sourceList) = sourceList |> Seq.map (fun source -> this.Build(source))
    
    member __.Build(source : 'TSource, destination : 'TDestination ref) = 
        mapper.Map(source, destination)
    
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination) = 
        async { return this.Build(source, ref destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = async { return this.BuildList(sourceList) } |> Async.StartAsTask

[<AbstractClass>]
[<Sealed>]
type GenericBuilder() = 
    static member CreateGenericBuilder<'TBuilder, 'TSource, 'TDestination when 'TBuilder :> BaseBuilder<'TSource, 'TDestination>>() = 
        let findCustomBuilderType (builderType : Type) (assembly : Assembly) = 
            match assembly.GetTypes() |> Seq.tryFind (fun t -> t.BaseType = builderType) with
            | Some(value) -> value
            | None -> 
                let assemblies = 
                    assembly.GetReferencedAssemblies()
                    |> Seq.sortBy (fun t -> t.Name)
                    |> Seq.map (fun t -> Assembly.Load(t))
                    |> Seq.collect (fun t -> t.GetTypes() |> Seq.filter (fun x -> x.BaseType = builderType))
                query { 
                    for refAssembly in assemblies do
                        select refAssembly
                        exactlyOneOrDefault
                }

        let builder = 
            ((match typeof<'TBuilder>.IsGenericTypeDefinition with
              | true -> typeof<'TBuilder>.MakeGenericType(typeof<'TSource>, typeof<'TDestination>)
              | _ -> typeof<'TBuilder>)
             |> Activator.CreateInstance) :?> 'TBuilder
        
        let builderType = builder.GetType()

        let customBuilder = 
            match Assembly.GetAssembly(typeof<'TDestination>) |> findCustomBuilderType builderType with
            | null -> 
                (match Assembly.GetEntryAssembly() with
                 | null -> Assembly.GetCallingAssembly()
                 | _ -> Assembly.GetEntryAssembly())
                |> findCustomBuilderType builderType
            | customBuilderType -> customBuilderType
        
        let rightBuilder = 
            match customBuilder with
            | null -> builder
            | _ -> Activator.CreateInstance(customBuilder) :?> 'TBuilder

        rightBuilder
    
    static member Create<'TSource, 'TDestination when 'TSource : equality and 'TDestination : equality and 'TSource : null and 'TDestination : null>() = 
        GenericBuilder.CreateGenericBuilder<BaseBuilder<'TSource, 'TDestination>, 'TSource, 'TDestination>()