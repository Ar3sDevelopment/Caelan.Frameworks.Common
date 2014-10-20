namespace Caelan.Frameworks.Common.Classes

open Caelan.Frameworks.Common.Interfaces

[<AbstractClass; Sealed>]
type Builder<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(mapper : IMapper<'TSource, 'TDestination>) = 
    member __.Build(source : 'TSource) = mapper.Map(source)
    member this.BuildList(sourceList) = sourceList |> Seq.map (fun source -> this.Build(source))
    member __.Build(source : 'TSource, destination : 'TDestination ref) = mapper.Map(source, destination)
    member this.BuildAsync(source) = async { return this.Build(source) } |> Async.StartAsTask
    member this.BuildAsync(source, destination) = 
        async { return this.Build(source, ref destination) } |> Async.StartAsTask
    member this.BuildListAsync(sourceList) = async { return this.BuildList(sourceList) } |> Async.StartAsTask
