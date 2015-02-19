namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open Autofac
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

[<Sealed>]
type Builder<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'TSource, 'TDestination>) = 
    static member internal Create() = Builder<'TSource, 'TDestination>.Create([ Assembly.GetCallingAssembly() ])
    static member internal Create(assemblies : seq<Assembly>) = Builder<'TSource, 'TDestination>(assemblies)
    static member internal Create(mapper : IMapper<'TSource, 'TDestination>) = Builder<'TSource, 'TDestination>(mapper)
    member __.Build(source) = mapper.Map(source)
    member this.BuildList(sourceList) = sourceList |> Seq.toArray |> Array.Parallel.map (fun source -> this.Build(source))
    member __.Build(source, destination : 'TDestination byref) = mapper.Map(source, ref destination)
    member __.Build(source, destination : 'TDestination) = mapper.Map(source, destination)
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination : 'TDestination byref) =
        let d = ref destination
        async { return this.Build(source, ref d.Value) } |> Async.StartAsTask
    member this.BuildAsync(source, destination : 'TDestination) = async { return this.Build(source, destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = async { return this.BuildList(sourceList) } |> Async.StartAsTask
    
    private new(assemblies) = 
        let cb = ContainerBuilder()
        let mapperType = typeof<IMapper<'TSource, 'TDestination>>
        cb.RegisterAssemblyTypes(assemblies
                                 |> Seq.filter (fun t -> t <> null)
                                 |> Seq.distinct
                                 |> Seq.toArray)
          .Where(fun t -> 
          not t.IsAbstract && not t.IsInterface && not t.IsGenericTypeDefinition && mapperType.IsAssignableFrom(t))
          .AsImplementedInterfaces()
        |> ignore
        cb.RegisterType<DefaultMapper<'TSource, 'TDestination>>().As<IMapper<'TSource, 'TDestination>>().PreserveExistingDefaults() |> ignore
        let container = cb.Build()
        let finalMapper = 
            using (container.BeginLifetimeScope()) (fun scope -> container.Resolve<IMapper<'TSource, 'TDestination>>())
        Builder<'TSource, 'TDestination>(finalMapper)
    
    private new() = Builder<'TSource, 'TDestination>([])

[<Sealed>]
type Builder<'T when 'T : equality and 'T : null and 'T : not struct> internal (assemblies : seq<Assembly>) = 
    member __.Destination<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
        Builder<'T, 'TDestination>.Create(assemblies |> Seq.append [ typeof<'T>.Assembly ])
    member __.Destination<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = 
        Builder<'T, 'TDestination>.Create(mapper)

[<Sealed; AbstractClass>]
type Builder private () = 
    static member Source<'T when 'T : equality and 'T : null and 'T : not struct>() = 
        Builder<'T>([ Assembly.GetCallingAssembly()
                      Assembly.GetExecutingAssembly()
                      typeof<'T>.Assembly ])
