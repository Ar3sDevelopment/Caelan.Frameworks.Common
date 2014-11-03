namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open Caelan.Frameworks.Common.Interfaces

[<Sealed>]
type Builder<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'TSource, 'TDestination>) = 
    static member Create() = Builder<'TSource, 'TDestination>()
    static member Create(mapper : IMapper<'TSource, 'TDestination>) = Builder<'TSource, 'TDestination>(mapper)
    member __.Build(source : 'TSource) = mapper.Map(source)
    member this.BuildList(sourceList) = sourceList |> Seq.map (fun source -> this.Build(source))
    member __.Build(source : 'TSource, destination : 'TDestination byref) = mapper.Map(source, ref destination)
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination) = async { return this.Build(source, destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = async { return this.BuildList(sourceList) } |> Async.StartAsTask
    new() = 
        let findMapper (assembly : Assembly) = 
            match assembly with
            | null -> None
            | _ -> 
                let baseMapper = typeof<IMapper<'TSource, 'TDestination>>
                match assembly.GetTypes() 
                      |> Seq.tryFind 
                             (fun t -> baseMapper.IsAssignableFrom(t) && t.IsInterface = false && t.IsAbstract = false) with
                | Some(assemblyMapper) -> 
                    Some(Activator.CreateInstance(assemblyMapper) :?> IMapper<'TSource, 'TDestination>)
                | None -> None
        
        let rec findMapperInAssemblies assembliesList = 
            match assembliesList with
            | head :: tail -> 
                match head |> findMapper with
                | Some(validMapper) -> validMapper
                | None -> tail |> findMapperInAssemblies
            | [] -> 
                { new IMapper<'TSource, 'TDestination> with
                      member __.Map(_) = Activator.CreateInstance(typeof<'TDestination>) :?> 'TDestination
                      member x.Map(source, destination) = destination <- x.Map(source) }
        
        let assemblies = 
            [ Assembly.GetExecutingAssembly()
              Assembly.GetEntryAssembly()
              Assembly.GetCallingAssembly()
              (typeof<'TSource>).Assembly
              (typeof<'TDestination>).Assembly ]
        
        let allAssemblies = 
            assemblies
            |> Seq.filter (fun t -> t <> null)
            |> Seq.collect (fun t -> t.GetReferencedAssemblies() |> Seq.map (fun x -> Assembly.Load(x)))
            |> Seq.append assemblies
            |> Seq.toList
        
        let finalMapper = allAssemblies |> findMapperInAssemblies
        Builder<'TSource, 'TDestination>(finalMapper)
