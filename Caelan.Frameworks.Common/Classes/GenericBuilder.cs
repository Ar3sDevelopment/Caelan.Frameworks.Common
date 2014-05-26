using AutoMapper;

namespace Caelan.Frameworks.Common.Classes
{
	public static class GenericBuilder
	{
		public static BaseBuilder<TSource, TDestination> Create<TSource, TDestination>()
			where TDestination : class, new()
			where TSource : class, new()
		{
			var builder = new BaseBuilder<TSource, TDestination>();

			if (Mapper.FindTypeMapFor<TSource, TDestination>() == null) Mapper.AddProfile(builder);

			return builder;
		}
	}
}
