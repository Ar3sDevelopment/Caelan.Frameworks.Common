﻿namespace Caelan.Frameworks.Common.Classes

open System
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Enums

[<AbstractClass>]
type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
    
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source : 'TSource, destination : 'TDestination byref, mapType : MapType) = 
            this.Map(source, ref destination, mapType)
        member this.Map(source : 'TSource, destination : 'TDestination byref) = this.Map(source, ref destination)
        member this.Map(source) = 
            match source with
            | null -> Unchecked.defaultof<'TDestination>
            | _ -> 
                let destination = Activator.CreateInstance<'TDestination>()
                (this :> IMapper<'TSource, 'TDestination>).Map(source, ref destination, MapType.NewObject)
                destination
    
    abstract Map : source:'TSource * destination:'TDestination byref -> unit
    override this.Map(source, destination : 'TDestination byref) = 
        (this :> IMapper<'TSource, 'TDestination>).Map(source, ref destination, MapType.EditObject)
    abstract Map : source:'TSource -> 'TDestination
    override this.Map(source) = (this :> IMapper<'TSource, 'TDestination>).Map(source)
    abstract Map : source:'TSource * destination:'TDestination byref * mapType:MapType -> unit