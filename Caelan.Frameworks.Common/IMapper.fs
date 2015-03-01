﻿namespace Caelan.Frameworks.Common.Interfaces

type IMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct> = 
    abstract Map : 'TSource -> 'TDestination
    abstract Map : source: 'TSource * destination:'TDestination -> unit