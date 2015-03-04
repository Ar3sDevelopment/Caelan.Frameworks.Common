namespace Caelan.Frameworks.Common.Classes

open Autofac
open Caelan.Frameworks.Common.Helpers
open Caelan.Frameworks.Common.Interfaces
open System
open System.Reflection

module Builder = 
    let internal isMapper (mapperType : Type) (t : Type) = mapperType.IsAssignableFrom(t) && not t.IsAbstract && not t.IsInterface && not t.IsGenericTypeDefinition
    
    let internal getMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : not struct and 'TDestination : equality and 'TDestination : not struct> (assemblies : Assembly []) = 
        typeof<IMapper<'TSource, 'TDestination>> |> MemoizeHelper.Memoize(fun mapperType -> 
                                                        let cb = ContainerBuilder()
                                                        let mainAssemblies = assemblies |> Array.filter (fun t -> t <> null)
                                                        let refAssemblies = mainAssemblies |> Array.collect (fun i -> i.GetReferencedAssemblies() |> Array.map Assembly.Load)
                                                        
                                                        let allAssemblies = 
                                                            mainAssemblies
                                                            |> Array.append refAssemblies
                                                            |> Array.filter (fun a -> a.GetTypes() |> Array.exists (fun i -> i |> isMapper mapperType))
                                                        cb.RegisterGeneric(typedefof<DefaultMapper<'TSource, 'TDestination>>).As(typedefof<IMapper<'TSource, 'TDestination>>) |> ignore
                                                        cb.RegisterAssemblyTypes(allAssemblies).Where(fun t -> t |> isMapper mapperType).AsImplementedInterfaces() |> ignore
                                                        let container = cb.Build()
                                                        using (container.BeginLifetimeScope()) (fun _ -> container.Resolve<IMapper<'TSource, 'TDestination>>()))
    
    [<Sealed>]
    type Builder<'T when 'T : equality and 'T : not struct> internal (source, assemblies : Assembly []) = 
        
        member this.To<'TDestination when 'TDestination : equality and 'TDestination : not struct>() = 
            let destination = Activator.CreateInstance<'TDestination>()
            let mapper = 
                assemblies
                |> Array.append [| typeof<'TDestination>.Assembly |]
                |> getMapper
            (destination, mapper) |> this.To
            destination
        
        member __.To<'TDestination when 'TDestination : equality and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) =
            let destination = Activator.CreateInstance<'TDestination>()
            (source, destination) |> mapper.Map
            destination
        
        member this.To<'TDestination when 'TDestination : equality and 'TDestination : not struct>(destination : 'TDestination) = 
            (destination, 
             assemblies
             |> Array.append [| typeof<'TDestination>.Assembly |]
             |> getMapper)
            |> this.To
        
        member __.To<'TDestination when 'TDestination : equality and 'TDestination : not struct>(destination, mapper : IMapper<'T, 'TDestination>) = 
            (source, destination) |> mapper.Map
    
    [<Sealed>]
    type ListBuilder<'T when 'T : equality and 'T : not struct> internal (sourceList, assemblies : Assembly []) = 
        
        member this.ToList<'TDestination when 'TDestination : equality and 'TDestination : not struct>() = 
            let allAssemblies = assemblies |> Array.append [| typeof<'TDestination>.Assembly |]
            getMapper<'T, 'TDestination> allAssemblies |> this.ToList
        
        member __.ToList<'TDestination when 'TDestination : equality and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = sourceList |> Seq.map mapper.Map
    
    let Build<'T when 'T : equality and 'T : not struct>(source : 'T) = 
        let assemblies = 
            [| Assembly.GetCallingAssembly()
               Assembly.GetExecutingAssembly()
               Assembly.GetEntryAssembly()
               AssemblyHelper.GetWebEntryAssembly()
               typeof<'T>.Assembly |]
        Builder<'T>(source, assemblies)
    
    let BuildList<'T when 'T : equality and 'T : not struct>(sourceList : seq<'T>) = 
        let assemblies = 
            [| Assembly.GetCallingAssembly()
               Assembly.GetExecutingAssembly()
               Assembly.GetEntryAssembly()
               AssemblyHelper.GetWebEntryAssembly()
               typeof<'T>.Assembly |]
        ListBuilder<'T>(sourceList, assemblies)
