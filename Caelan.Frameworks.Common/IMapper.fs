namespace Caelan.Frameworks.Common.Interfaces

type IMapper<'TSource, 'TDestination when 'TSource : equality and 'TDestination : equality and 'TSource : null and 'TDestination : null> = 
    abstract Map : 'TSource -> 'TDestionation
    abstract Map : 'TSource * 'TDestionation ref -> unit