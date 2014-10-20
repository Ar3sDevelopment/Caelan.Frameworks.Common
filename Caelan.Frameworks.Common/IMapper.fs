namespace Caelan.Frameworks.Common.Interfaces

open System

type IMapper<'TSource, 'TDestination> = 
    abstract Map : 'TSource -> 'TDestionation
    abstract Map : 'TSource * 'TDestionation ref -> unit