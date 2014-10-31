namespace Caelan.Frameworks.Common.Classes

open Caelan.Frameworks.Common.Interfaces

[<AbstractClass>]
type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
    
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source : 'TSource, destination : 'TDestination ref) = this.Map(source, destination)
        member this.Map(source) = 
            let refDest : 'TDestination ref = ref null
            (this :> IMapper<'TSource, 'TDestination>).Map(source, refDest)
            !refDest
    
    abstract Map : source:'TSource * destination:'TDestination ref -> unit
    abstract Map : source:'TSource -> 'TDestination
    override this.Map(source) = (this :> IMapper<'TSource, 'TDestination>).Map(source)
