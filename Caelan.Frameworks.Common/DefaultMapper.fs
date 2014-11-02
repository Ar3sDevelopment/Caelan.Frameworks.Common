namespace Caelan.Frameworks.Common.Classes

open System
open Caelan.Frameworks.Common.Interfaces

[<AbstractClass>]
type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
    
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source : 'TSource, destination : 'TDestination byref) = this.Map(source, ref destination)
        member this.Map(source) = 
            let destination = Activator.CreateInstance<'TDestination>()
            (this :> IMapper<'TSource, 'TDestination>).Map(source, ref destination)
            destination
    
    abstract Map : source:'TSource * destination:'TDestination byref -> unit
    abstract Map : source:'TSource -> 'TDestination
    override this.Map(source) = (this :> IMapper<'TSource, 'TDestination>).Map(source)
