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

        abstract member AddMappingConfigurations : IMappingExpression<'TSource, 'TDestination> -> unit
        default this.AddMappingConfigurations(mappingExpression : IMappingExpression<'TSource, 'TDestination>) =
            AutoMapperExtender.IgnoreAllNonExisting(mappingExpression) |> ignore

        abstract member AfterBuild : 'TSource * 'TDestination ref -> unit
        default this.AfterBuild(source : 'TSource, destination : 'TDestination ref) =
            ()

        member this.Build(source : 'TSource) =
            match source with
            | null -> Unchecked.defaultof<'TDestination>
            | _ ->
                let mutable dest = Unchecked.defaultof<'TDestination>
        
                if (box dest = null) then
                    dest <- Activator.CreateInstance<'TDestination>()

                this.Build(source, ref dest)

                dest

        member this.BuildList(sourceList : seq<'TDestination>) =
            sourceList |> Seq.map (fun source -> this.Build(unbox source))

        member this.Build(source : 'TSource, destination : 'TDestination byref) =
            if box source = null || source.Equals(Unchecked.defaultof<'TSource>) then
                destination <- Unchecked.defaultof<'TDestination>
            else
                destination <- Mapper.DynamicMap<'TSource, 'TDestination>(source)
            ()

        override this.Configure() =
            base.Configure()

            let mappingExpression = Mapper.CreateMap<'TSource, 'TDestination>()
            mappingExpression.AfterMap(fun source destination ->
                let refDest = ref destination
                this.AfterBuild(source,refDest) |> ignore
            ) |> ignore

            this.AddMappingConfigurations(mappingExpression)

    [<AbstractClass; Sealed>]
    type GenericBuilder() =
        static member CreateGenericBuilder<'TBuilder, 'TSource, 'TDestination when 'TBuilder :> BaseBuilder<'TSource, 'TDestination>>() =
            let builderType = typedefof<'TBuilder>
            let builder =
                match builderType.IsGenericType with
                | true -> Activator.CreateInstance(builderType.MakeGenericType(typedefof<'TSource>, typedefof<'TDestination>)) :?> 'TBuilder
                | _ -> Activator.CreateInstance<'TBuilder>()

            let mutable assembly = Assembly.GetAssembly(typedefof<'TDestination>)
            let mutable customBuilder =
                match assembly.GetTypes() |> Array.tryFind (fun t -> t.BaseType = builder.GetType()) with
                | None -> null
                | Some(value) -> value

            if customBuilder = null then
                let referAssemblies = assembly.GetReferencedAssemblies() |> Array.sortBy (fun t -> t.Name)
                customBuilder <- referAssemblies.Select(fun t -> Assembly.Load(t)).SelectMany(fun t -> t.GetTypes().Where(fun x -> x.BaseType = builder.GetType())).SingleOrDefault()

            if customBuilder = null then
                assembly <-
                    match Assembly.GetEntryAssembly() with
                    | null -> Assembly.GetCallingAssembly()
                    | _ -> Assembly.GetEntryAssembly()

                customBuilder <-
                    match assembly.GetTypes() |> Array.tryFind (fun t -> t.BaseType = builder.GetType()) with
                    | None -> null
                    | Some(value) -> value

                if customBuilder = null then
                    let referAssemblies = assembly.GetReferencedAssemblies() |> Array.sortBy (fun t -> t.Name)
                    customBuilder <- referAssemblies.Select(fun t -> Assembly.Load(t)).SelectMany(fun t -> t.GetTypes().Where(fun x -> x.BaseType = builder.GetType())).SingleOrDefault()

                    if customBuilder = null then
                        if Mapper.FindTypeMapFor<'TSource, 'TDestination>() = null then
                            Mapper.AddProfile(builder)

                        builder
                    else
                        Activator.CreateInstance(customBuilder) :?> 'TBuilder
                else
                    Activator.CreateInstance(customBuilder) :?> 'TBuilder
            else
                Activator.CreateInstance(customBuilder) :?> 'TBuilder

        static member Create<'TSource, 'TDestination when 'TSource : equality and 'TDestination : equality and 'TSource : null and 'TDestination : null>() =
            GenericBuilder.CreateGenericBuilder<BaseBuilder<'TSource, 'TDestination>, 'TSource, 'TDestination>()
