namespace Caelan.Frameworks.Common.Interfaces

type IMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct> = 
    abstract Source : 'TSource with set
    abstract Map : unit -> 'TDestination
    abstract Map : destination:'TDestination byref -> unit
    abstract Map : destination:'TDestination -> 'TDestination