namespace Caelan.Frameworks.Common.Classes

open Caelan.Frameworks.Common.Interfaces

[<AbstractClass>]
type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
    
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source : 'TSource, destination : 'TDestination ref) = this.Map(source, destination)
        member this.Map(source) = 
            let dest : 'TDestination = null
            let refDest = ref dest
            (this :> IMapper<'TSource, 'TDestination>).Map(source, refDest)
            !refDest
    
    abstract Map : 'TSource * 'TDestination ref -> unit
    member this.Map(source) = (this :> IMapper<'TSource, 'TDestination>).Map(source)
