namespace Caelan.Frameworks.Common.Interfaces

type IMapper<'TSource, 'TDestination> = 
    abstract Map : 'TSource -> 'TDestionation
    abstract Map : 'TSource * 'TDestionation ref -> unit