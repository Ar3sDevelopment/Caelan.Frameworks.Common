namespace Caelan.Frameworks.Common.Modules
open System
open System.Reflection
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

module internal MapperReflection = 
    let GetMapper assemblies =
        let findMapper (assembly : Assembly) = 
            let mapper =
                assembly.GetTypes()
                |> Seq.tryFind (fun t -> not t.IsInterface && not t.IsAbstract && typeof<IMapper<'TSource, 'TDestination>>.IsAssignableFrom(t))
            match mapper with
            | Some(m) -> Some(Activator.CreateInstance(m) :?> IMapper<'TSource, 'TDestination>)
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
        
        let allAssemblies = 
            assemblies
            |> Seq.append ([ Assembly.GetExecutingAssembly()
                             Assembly.GetEntryAssembly()
                             AssemblyHelper.GetWebEntryAssembly()
                             Assembly.GetCallingAssembly() ])
            |> Seq.filter (fun t -> t <> null)

        let refAssemblies = allAssemblies |> Seq.collect (fun t -> t.GetReferencedAssemblies() |> Seq.map (fun x -> Assembly.Load(x)))
        
        allAssemblies
        |> Seq.append refAssemblies
        |> Seq.toList
        |> findMapperInAssemblies