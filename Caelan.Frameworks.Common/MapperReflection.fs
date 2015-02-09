namespace Caelan.Frameworks.Common.Modules
open System
open System.Reflection
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers
open Caelan.Frameworks.Common.Extenders

[<Sealed; AbstractClass>]
type internal MapperReflection<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct> private() = 
    static member GetMapper assemblies =
        let findMapper (assembly : Assembly) = 
            let mapper =
                assembly.GetTypes()
                |> Seq.tryFind (fun t -> not t.IsInterface && not t.IsAbstract && typeof<IMapper<'TSource, 'TDestination>>.IsAssignableFrom(t))
            match mapper with
            | Some(m) -> Some(Activator.CreateInstance<IMapper<'TSource, 'TDestination>>(m))
            | None -> None
        
        let rec findMapperInAssemblies assemblies = 
            match assemblies with
            | head :: tail -> 
                match head |> findMapper with
                | Some(validMapper) -> validMapper
                | None -> tail |> findMapperInAssemblies
            | [] -> 
                { new IMapper<'TSource, 'TDestination> with
                      member __.Map(source) = 
                          Activator.CreateInstance<'TDestination>()
                      member x.Map(source, destination : 'TDestination byref) = destination <- x.Map(source) }
        
        let allAssemblies = 
            assemblies
            |> Seq.append ([ Assembly.GetExecutingAssembly()
                             Assembly.GetEntryAssembly()
                             AssemblyHelper.GetWebEntryAssembly()
                             Assembly.GetCallingAssembly() ])
            |> Seq.filter (fun t -> t <> null)

        let refAssemblies = allAssemblies |> Seq.toArray |> Array.Parallel.collect (fun t -> t.GetReferencedAssemblies() |> Array.Parallel.map (fun x -> Assembly.Load(x)))
        
        allAssemblies
        |> Seq.append refAssemblies
        |> Seq.toList
        |> findMapperInAssemblies