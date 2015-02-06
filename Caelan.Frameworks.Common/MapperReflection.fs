namespace Caelan.Frameworks.Common.Modules
open System
open System.Reflection
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

module internal MapperReflection = 
    let GetMapper (assemblies : seq<Assembly>) =
        let findMapper (assembly : Assembly) = 
            let baseMapper = typeof<IMapper<'TSource, 'TDestination>>
            match assembly.GetTypes() 
                  |> Seq.tryFind (fun t -> baseMapper.IsAssignableFrom(t) && not t.IsInterface && not t.IsAbstract) with
            | Some(assemblyMapper) -> 
                Some(Activator.CreateInstance(assemblyMapper) :?> IMapper<'TSource, 'TDestination>)
            | None -> None
        
        let rec findMapperInAssemblies assemblies = 
            match assemblies with
            | head :: tail -> 
                match head |> findMapper with
                | Some(validMapper) -> validMapper
                | None -> tail |> findMapperInAssemblies
            | [] -> 
                { new IMapper<'TSource, 'TDestination> with
                      member __.Map(source : 'TSource) = 
                          Activator.CreateInstance(typeof<'TDestination>) :?> 'TDestination
                      member x.Map(source, destination : 'TDestination byref) = destination <- x.Map(source) }
        
        let additionalAssemblies = 
            [ Assembly.GetExecutingAssembly()
              Assembly.GetEntryAssembly()
              AssemblyHelper.GetWebEntryAssembly()
              Assembly.GetCallingAssembly()
            ]
            |> Seq.filter (fun t -> t <> null)
        
        let allAssemblies = 
            assemblies
            |> Seq.append additionalAssemblies
            |> Seq.collect (fun t -> t.GetReferencedAssemblies() |> Seq.map (fun x -> Assembly.Load(x)))
            |> Seq.append assemblies
            |> Seq.append additionalAssemblies
        
        allAssemblies
        |> Seq.toList
        |> findMapperInAssemblies