using System;
using AutoMapper;
using Caelan.Frameworks.Common.Enums;
using Caelan.Frameworks.Common.Interfaces;
using Microsoft.FSharp.Core;

namespace Caelan.Frameworks.Common.AutoMapper.Mappers
{
	public class AutoMapperMapper<TSource, TDestination> : Profile, IMapper<TSource, TDestination>
		where TSource : class
		where TDestination : class
	{
		public TDestination Map(TSource source)
		{
			var destRef = Activator.CreateInstance<TDestination>();

			Map(source, ref destRef, MapType.NewObject);

			return destRef;
		}

		public void Map(TSource source, ref TDestination destination)
		{
			Map(source, ref destination, MapType.EditObject);
		}

		public void Map(TSource source, ref TDestination destination, MapType mapType)
		{
			switch (mapType)
			{
				case MapType.NewObject:
					break;
				case MapType.EditObject:
					if (destination != null)
						Mapper.Map(source, destination);
					else
						destination = Mapper.Map<TSource, TDestination>(source);
					break;
			}
		}
	}
}
