namespace Caelan.Frameworks.Common.Interfaces

type IMapper<'TSource, 'TDestination> = 
    abstract Map : 'TSource -> 'TDestination
    abstract Map : source: 'TSource * destination:'TDestination -> 'TDestination