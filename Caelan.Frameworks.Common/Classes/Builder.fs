namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

[<Sealed>]
type Builder<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'TSource, 'TDestination>) = 
    static member Create() = Builder<'TSource, 'TDestination>.Create([ Assembly.GetCallingAssembly() ])
    static member internal Create(assemblies: seq<Assembly>) = Builder<'TSource, 'TDestination>(assemblies)
    static member Create(mapper : IMapper<'TSource, 'TDestination>) = Builder<'TSource, 'TDestination>(mapper)
    member __.Build(source : 'TSource) = mapper.Map(source)
    member __.BuildList(source : 'TSource) = mapper.Map(source)
    member this.BuildList(sourceList : seq<'TSource>) = sourceList |> Seq.map (fun source -> this.BuildList(source))
    member __.Build(source : 'TSource, destination : 'TDestination byref) = mapper.Map(source, ref destination)
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination) = async { return this.Build(source, destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList : seq<'TSource>) = async { return this.BuildList(sourceList) } |> Async.StartAsTask
    private new(assemblies: seq<Assembly>) =
        let findMapper (assembly : Assembly) = 
            let baseMapper = typeof<IMapper<'TSource, 'TDestination>>
            match assembly.GetTypes() |> Seq.tryFind (fun t -> baseMapper.IsAssignableFrom(t) && not t.IsInterface && not t.IsAbstract) with
            | Some(assemblyMapper) -> Some(Activator.CreateInstance(assemblyMapper) :?> IMapper<'TSource, 'TDestination>)
            | None -> None
        
        let rec findMapperInAssemblies assemblies = 
            match assemblies with
            | head :: tail -> 
                match head |> findMapper with
                | Some(validMapper) -> validMapper
                | None -> tail |> findMapperInAssemblies
            | [] -> 
                { new IMapper<'TSource, 'TDestination> with
                      member __.Map(source : 'TSource) = Activator.CreateInstance(typeof<'TDestination>) :?> 'TDestination
                      member x.Map(source, destination : 'TDestination byref) = destination <- x.Map(source) }
        
        let allAssemblies = 
            assemblies
            |> Seq.collect (fun t -> t.GetReferencedAssemblies() |> Seq.map (fun x -> Assembly.Load(x)))
            |> Seq.append assemblies
        
        let finalMapper = 
            allAssemblies
            |> Seq.toList
            |> findMapperInAssemblies
        
        Builder<'TSource, 'TDestination>(finalMapper)

    new() = 
        let assemblies = 
            [ Assembly.GetExecutingAssembly()
              Assembly.GetEntryAssembly()
              AssemblyHelper.GetWebEntryAssembly()
              Assembly.GetCallingAssembly()
              typeof<'TSource>.Assembly
              typeof<'TDestination>.Assembly ]
            |> Seq.filter (fun t -> t <> null)

        Builder<'TSource, 'TDestination>(assemblies)

[<Sealed>]
type Builder<'T when 'T : equality and 'T : null and 'T : not struct> internal (assemblies: seq<Assembly>) =
    member __.Destination<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = Builder<'T, 'TDestination>.Create(assemblies)
    member __.Destination<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = Builder<'T, 'TDestination>.Create(mapper)

[<Sealed>]
type Builder private () =
    static member Source<'T when 'T : equality and 'T : null and 'T : not struct>() = Builder<'T>([ Assembly.GetCallingAssembly() ])