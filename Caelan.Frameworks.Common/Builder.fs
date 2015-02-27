namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open Autofac
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

[<Sealed>]
type Builder<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(source : 'TSource, mapper : IMapper<'TSource, 'TDestination>) = 
    static member internal Create(source) = 
        Builder<'TSource, 'TDestination>.Create(source, Seq.singleton (Assembly.GetCallingAssembly()))
    static member internal Create(source : 'TSource, assemblies : seq<Assembly>) = Builder<'TSource, 'TDestination>(source, assemblies)
    static member internal Create(source : 'TSource, mapper : IMapper<'TSource, 'TDestination>) = Builder<'TSource, 'TDestination>(source, mapper)
    member __.Build() = mapper.Map()
    
    member this.BuildList(sourceList) = 
        sourceList
        |> Seq.toArray
        |> Array.Parallel.map (fun source -> this.Build())
        |> Seq.ofArray
    
    member __.Build(source, destination : 'TDestination byref) = mapper.Map(ref destination)
    member __.Build(source, destination : 'TDestination) = mapper.Map(destination)
    member this.BuildAsync(source) = async { return this.Build() } |> Async.StartAsTask
    
    member this.BuildAsync(source, destination : 'TDestination byref) = 
        let d = ref destination
        async { return this.Build(source, ref d.Value) } |> Async.StartAsTask
    
    member this.BuildAsync(source, destination : 'TDestination) = 
        async { return this.Build(source, destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = async { return this.BuildList(sourceList) } |> Async.StartAsTask
    
    private new(source : 'TSource, assemblies) = 
        let cb = ContainerBuilder()
        let mapperType = typeof<IMapper<'TSource, 'TDestination>>
        cb.RegisterGeneric(typedefof<DefaultMapper<'TSource, 'TDestination>>)
          .As(typedefof<IMapper<'TSource, 'TDestination>>) |> ignore
        let mainAssemblies = 
            assemblies
            |> Seq.filter (fun t -> t <> null)
            |> Seq.distinct
            |> Seq.toArray
        
        let refAssemblies = 
            mainAssemblies
            |> Array.Parallel.collect (fun i -> i.GetReferencedAssemblies())
            |> Array.Parallel.map (fun t -> Assembly.Load(t))
        
        cb.RegisterAssemblyTypes(mainAssemblies |> Array.append refAssemblies)
          .Where(fun t -> 
          not t.IsAbstract && not t.IsInterface && not t.IsGenericTypeDefinition && mapperType.IsAssignableFrom(t))
          .AsImplementedInterfaces() |> ignore
        let container = cb.Build()
        let finalMapper = 
            using (container.BeginLifetimeScope()) (fun scope -> container.Resolve<IMapper<'TSource, 'TDestination>>(NamedParameter("source", source)))
        Builder<'TSource, 'TDestination>(source, finalMapper)
    
    private new(source : 'TSource) = Builder<'TSource, 'TDestination>(source, Seq.empty<Assembly>)

[<Sealed>]
type Builder<'T when 'T : equality and 'T : null and 'T : not struct> internal (source, assemblies : seq<Assembly>) = 
    member __.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
        Builder<'T, 'TDestination>.Create(source, assemblies |> Seq.append (Seq.singleton typeof<'TDestination>.Assembly))
    member __.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = 
        Builder<'T, 'TDestination>.Create(source, mapper)

[<Sealed; AbstractClass>]
type Builder private () = 
    static member Build<'T when 'T : equality and 'T : null and 'T : not struct>(source : 'T) = 
        Builder<'T>(source, [ Assembly.GetCallingAssembly()
                              Assembly.GetExecutingAssembly()
                              Assembly.GetEntryAssembly()
                              AssemblyHelper.GetWebEntryAssembly()
                              typeof<'T>.Assembly ])
