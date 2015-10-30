namespace Caelan.Frameworks.Common.Interfaces

type IMapper<'TSource, 'TDestination> = 
    /// <summary>
    /// 
    /// </summary>
    abstract Map : 'TSource -> 'TDestination
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    abstract Map : source: 'TSource * destination:'TDestination -> 'TDestination