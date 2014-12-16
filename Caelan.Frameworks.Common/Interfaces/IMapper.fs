namespace Caelan.Frameworks.Common.Interfaces

open Caelan.Frameworks.Common.Enums

[<AllowNullLiteral>]
type IMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct> = 
    abstract Map : 'TSource -> 'TDestination
    abstract Map : 'TSource * 'TDestination byref -> unit