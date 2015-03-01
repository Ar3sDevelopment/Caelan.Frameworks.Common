namespace Caelan.Frameworks.Common.Classes

open Autofac
open Caelan.Frameworks.Common.Helpers
open Caelan.Frameworks.Common.Interfaces
open System
open System.Reflection

module BuilderModule = 
    let internal isMapper (mapperType : Type) (t : Type) = not t.IsAbstract && not t.IsInterface && not t.IsGenericTypeDefinition && mapperType.IsAssignableFrom(t)
    
    let internal GetMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(assemblies : Assembly []) = 
        let cb = ContainerBuilder()
        let mapperType = typeof<IMapper<'TSource, 'TDestination>>
        let mainAssemblies = assemblies |> Array.filter (fun t -> t <> null)
        let refAssemblies = mainAssemblies |> Array.Parallel.collect (fun i -> i.GetReferencedAssemblies() |> Array.Parallel.map Assembly.Load)
        
        let allAssemblies = 
            mainAssemblies
            |> Array.append refAssemblies
            |> Array.Parallel.choose (fun a -> 
                   let t = a.GetTypes() |> Seq.tryFind (fun i -> i |> isMapper mapperType)
                   match t with
                   | None -> None
                   | _ -> Some a)
        cb.RegisterGeneric(typedefof<DefaultMapper<'TSource, 'TDestination>>).As(typedefof<IMapper<'TSource, 'TDestination>>) |> ignore
        cb.RegisterAssemblyTypes(allAssemblies).Where(fun t -> t |> isMapper mapperType).AsImplementedInterfaces() |> ignore
        let container = cb.Build()
        let finalMapper = using (container.BeginLifetimeScope()) (fun _ -> container.Resolve<IMapper<'TSource, 'TDestination>>())
        finalMapper

[<Sealed>]
type Builder<'T when 'T : equality and 'T : null and 'T : not struct> internal (source, assemblies : Assembly []) = 
    
    member this.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
        let allAssemblies = assemblies |> Array.append [| typeof<'TDestination>.Assembly |]
        BuilderModule.GetMapper<'T, 'TDestination> allAssemblies |> this.To
    
    member __.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = mapper.Map(source)
    
    member this.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(destination : 'TDestination) = 
        let allAssemblies = assemblies |> Array.append [| typeof<'TDestination>.Assembly |]
        (destination, BuilderModule.GetMapper allAssemblies) |> this.To
    
    member __.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(destination, mapper : IMapper<'T, 'TDestination>) = 
        (source, destination) |> mapper.Map

[<Sealed>]
type ListBuilder<'T when 'T : equality and 'T : null and 'T : not struct> internal (sourceList, assemblies : Assembly []) = 
    
    member this.ToList<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
        let allAssemblies = assemblies |> Array.append [| typeof<'TDestination>.Assembly |]
        BuilderModule.GetMapper<'T, 'TDestination> allAssemblies |> this.ToList
    
    member __.ToList<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = sourceList |> Seq.map mapper.Map

module Builder = 
    let Build<'T when 'T : equality and 'T : null and 'T : not struct>(source : 'T) = 
        let assemblies = 
            [| Assembly.GetCallingAssembly()
               Assembly.GetExecutingAssembly()
               Assembly.GetEntryAssembly()
               AssemblyHelper.GetWebEntryAssembly()
               typeof<'T>.Assembly |]
        Builder<'T>(source, assemblies)
    
    let BuildList<'T when 'T : equality and 'T : null and 'T : not struct>(sourceList : seq<'T>) = 
        let assemblies = 
            [| Assembly.GetCallingAssembly()
               Assembly.GetExecutingAssembly()
               Assembly.GetEntryAssembly()
               AssemblyHelper.GetWebEntryAssembly()
               typeof<'T>.Assembly |]
        ListBuilder<'T>(sourceList, assemblies)
