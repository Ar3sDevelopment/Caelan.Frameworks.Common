namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open Autofac
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

[<Sealed; AbstractClass>]
type Builder private () = 
    static member internal GetMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(source: 'TSource, assemblies : seq<Assembly>) =
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
        finalMapper

[<Sealed>]
type Builder<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct> internal (mapper: IMapper<'TSource, 'TDestination>) = 
    member __.Build() = mapper.Map()

    member __.Build(destination : 'TDestination) = mapper.Map(destination)
    member this.BuildAsync() = async { return this.Build() } |> Async.StartAsTask
    
    member this.BuildAsync(destination : 'TDestination) = 
        async { return this.Build(destination) } |> Async.StartAsTask

[<Sealed>]
type Builder<'T when 'T : equality and 'T : null and 'T : not struct> internal (source, assemblies : seq<Assembly>) = 
    member this.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
        let allAssemblies = assemblies |> Seq.append (Seq.singleton typeof<'TDestination>.Assembly)
        Builder<'T, 'TDestination>(Builder.GetMapper(source, allAssemblies)).Build()
    member __.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = 
        Builder<'T, 'TDestination>(mapper).Build()
    member this.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(destination) = 
        let allAssemblies = assemblies |> Seq.append (Seq.singleton typeof<'TDestination>.Assembly)
        Builder<'T, 'TDestination>(Builder.GetMapper(source, allAssemblies)).Build(destination)
    member __.To<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(destination, mapper : IMapper<'T, 'TDestination>) = 
        Builder<'T, 'TDestination>(mapper).Build(destination)

[<Sealed>]
type ListBuilder<'T when 'T : equality and 'T : null and 'T : not struct> internal (sourceList, assemblies : seq<Assembly>) = 
    member __.ToList<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
        let allAssemblies = assemblies |> Seq.append (Seq.singleton typeof<'TDestination>.Assembly)
        sourceList |> Seq.map (fun s -> Builder<'T, 'TDestination>(Builder.GetMapper(s, allAssemblies)).Build())
    member __.ToList<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = 
        sourceList |> Seq.map (fun s -> Builder<'T, 'TDestination>(mapper).Build())

type Builder with
    static member Build<'T when 'T : equality and 'T : null and 'T : not struct>(source : 'T) = 
        Builder<'T>(source, [ Assembly.GetCallingAssembly()
                              Assembly.GetExecutingAssembly()
                              Assembly.GetEntryAssembly()
                              AssemblyHelper.GetWebEntryAssembly()
                              typeof<'T>.Assembly ])
    static member BuildList<'T when 'T : equality and 'T : null and 'T : not struct>(sourceList : seq<'T>) =
        ListBuilder<'T>(sourceList, [ Assembly.GetCallingAssembly()
                                      Assembly.GetExecutingAssembly()
                                      Assembly.GetEntryAssembly()
                                      AssemblyHelper.GetWebEntryAssembly()
                                      typeof<'T>.Assembly ])
