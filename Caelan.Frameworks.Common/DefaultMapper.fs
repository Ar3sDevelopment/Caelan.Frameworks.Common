namespace Caelan.Frameworks.Common.Classes

open System
open Caelan.Frameworks.Common.Interfaces

[<AbstractClass>]
type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source, destination) = this.Map(source, ref destination)
        member this.Map(source) = this.Map(source)
    
    abstract Map : source:'TSource * destination:'TDestination byref -> unit
    abstract Map : source:'TSource -> 'TDestination
    override this.Map source = 
        match source with
        | null -> Unchecked.defaultof<_>
        | _ -> 
            let destination = Activator.CreateInstance<_>()
            this.Map(source, ref destination)
            destination
