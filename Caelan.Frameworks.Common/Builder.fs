namespace Caelan.Frameworks.Common.Classes

open System
open System.Reflection
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers
open Caelan.Frameworks.Common.Modules

[<Sealed>]
type Builder<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct> private (mapper : IMapper<'TSource, 'TDestination>) = 
    static member internal Create() = Builder<'TSource, 'TDestination>.Create([ Assembly.GetCallingAssembly() ])
    static member internal Create(assemblies : seq<Assembly>) = Builder<'TSource, 'TDestination>(assemblies)
    static member internal Create(mapper : IMapper<'TSource, 'TDestination>) = Builder<'TSource, 'TDestination>(mapper)
    member __.Build(source) = mapper.Map(source)
    member this.BuildList(sourceList) = sourceList |> Seq.map (fun source -> this.Build(source))
    member __.Build(source, destination) = mapper.Map(source, ref destination)
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination) = async { return this.Build(source, destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = 
        async { return this.BuildList(sourceList) } |> Async.StartAsTask
    
    private new(assemblies) = 
        let finalMapper =
            assemblies
            |> Seq.append [ typeof<'TSource>.Assembly
                            typeof<'TDestination>.Assembly ]
            |> MapperReflection<'TSource, 'TDestination>.GetMapper
        
        Builder<'TSource, 'TDestination>(finalMapper)
    
    private new() = Builder<'TSource, 'TDestination>([])

[<Sealed>]
type Builder<'T when 'T : equality and 'T : null and 'T : not struct> internal (assemblies : seq<Assembly>) = 
    member __.Destination<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
        Builder<'T, 'TDestination>.Create(assemblies)
    member __.Destination<'TDestination when 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'T, 'TDestination>) = 
        Builder<'T, 'TDestination>.Create(mapper)
    internal new() = 
        let assemblies = 
            [ Assembly.GetExecutingAssembly()
              Assembly.GetEntryAssembly()
              AssemblyHelper.GetWebEntryAssembly()
              Assembly.GetCallingAssembly()
              typeof<'T>.Assembly ]
            |> Seq.filter (fun t -> t <> null)
        Builder<'T>(assemblies)

[<Sealed; AbstractClass>]
type Builder private () = 
    static member Source<'T when 'T : equality and 'T : null and 'T : not struct>() = 
        Builder<'T>([ Assembly.GetCallingAssembly() ])
