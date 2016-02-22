namespace Caelan.Frameworks.ClassBuilder.Classes

open Autofac
open Caelan.Frameworks.ClassBuilder
open Caelan.Frameworks.Common.Helpers
open Caelan.Frameworks.ClassBuilder.Interfaces
open System
open System.Reflection

module Builder = 
    let mutable private container = ContainerBuilder().Build()
    let private isMapper (t : Type) = 
        typeof<IMapper>.IsAssignableFrom(t) && not t.IsAbstract && not t.IsInterface && not t.IsGenericTypeDefinition
    
    let internal getMapper<'TSource, 'TDestination> (assemblies : Assembly []) = 
        let mutable mapper = Unchecked.defaultof<IMapper<'TSource, 'TDestination>>
        use scope = container.BeginLifetimeScope()
        if container.TryResolve<IMapper<'TSource, 'TDestination>>(&mapper) |> not then 
            let mainAssemblies = assemblies |> Array.filter (isNull >> not)
            let refAssemblies = mainAssemblies |> Array.collect (fun i -> i.GetReferencedAssemblies() |> Array.map Assembly.Load)
            
            let allAssemblies = 
                mainAssemblies
                |> Array.append refAssemblies
                |> Array.filter (fun a -> a.GetTypes() |> Array.exists isMapper)
            
            let cb = ContainerBuilder()
            cb.RegisterGeneric(typedefof<DefaultMapper<'TSource, 'TDestination>>)
              .As(typedefof<IMapper<'TSource, 'TDestination>>) |> ignore
            cb.RegisterAssemblyTypes(allAssemblies).Where(fun t -> t |> isMapper).AsImplementedInterfaces() |> ignore
            cb.Update(container)
            mapper <- container.Resolve<IMapper<'TSource, 'TDestination>>()
        mapper
    
    let RegisterMapper<'TMapper when 'TMapper :> IMapper>() = 
        let cb = ContainerBuilder()
        cb.RegisterType<'TMapper>().AsImplementedInterfaces() |> ignore
        cb.Update(container) |> ignore
    
    let internal registerAssemblies (allAssemblies : Assembly []) = 
        let cb = ContainerBuilder()
        cb.RegisterAssemblyTypes(allAssemblies).Where(fun t -> t |> isMapper).AsImplementedInterfaces() |> ignore
        cb.Update(container) |> ignore
    
    [<Sealed>]
    type Builder<'T> internal (source, assemblies : Assembly []) = 
        
        /// <summary>
        /// 
        /// </summary>
        member this.To<'TDestination>() = 
            let destination = Activator.CreateInstance<'TDestination>()
            
            let mapper = 
                assemblies
                |> Array.append [| typeof<'TDestination>.Assembly |]
                |> getMapper<'T, 'TDestination>
            (destination, mapper) |> this.To
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        member this.To<'TDestination>(mapper : IMapper<'T, 'TDestination>) = 
            (Activator.CreateInstance<'TDestination>(), mapper) |> this.To
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        member this.To<'TDestination>(destination : 'TDestination) = 
            let mapper = 
                assemblies
                |> Array.append [| typeof<'TDestination>.Assembly |]
                |> getMapper<'T, 'TDestination>
            (destination, mapper) |> this.To
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="mapper"></param>
        member __.To<'TDestination>(destination, mapper : IMapper<'T, 'TDestination>) = 
            match source :> obj |> Option.ofObj with
            | None -> Unchecked.defaultof<'TDestination>
            | Some(s) -> (source, destination) |> mapper.Map
    
    [<Sealed>]
    type ListBuilder<'T> internal (sourceList, assemblies : Assembly []) = 
        
        /// <summary>
        /// 
        /// </summary>
        member this.ToList<'TDestination>() = 
            let allAssemblies = assemblies |> Array.append [| typeof<'TDestination>.Assembly |]
            allAssemblies
            |> getMapper<'T, 'TDestination>
            |> this.ToList
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        member __.ToList<'TDestination>(mapper : IMapper<'T, 'TDestination>) = sourceList |> Seq.map mapper.Map
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    let Build<'T>(source : 'T) = 
        let assemblies = 
            [| Assembly.GetCallingAssembly()
               Assembly.GetExecutingAssembly()
               Assembly.GetEntryAssembly()
               AssemblyHelper.GetWebEntryAssembly()
               typeof<'T>.Assembly |]
        Builder<'T>(source, assemblies)
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceList"></param>
    let BuildList<'T>(sourceList : seq<'T>) = 
        let assemblies = 
            [| Assembly.GetCallingAssembly()
               Assembly.GetExecutingAssembly()
               Assembly.GetEntryAssembly()
               AssemblyHelper.GetWebEntryAssembly()
               typeof<'T>.Assembly |]
        ListBuilder<'T>(sourceList, assemblies)
