using AutoMapper;
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
			TDestination dest = null;
			var destRef = new FSharpRef<TDestination>(dest);

			Map(source, destRef);

			return destRef.Value;
		}

		public void Map(TSource source, FSharpRef<TDestination> destination)
		{
			Mapper.Map(source, destination.Value);
		}
	}
}
