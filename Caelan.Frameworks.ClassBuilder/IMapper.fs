namespace Caelan.Frameworks.ClassBuilder.Interfaces

type IMapper =
    interface
    end

type IMapper<'TSource, 'TDestination> = 
    inherit IMapper
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