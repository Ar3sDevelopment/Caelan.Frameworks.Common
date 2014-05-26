using System;
using System.Linq;
using System.Reflection;
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

            var customBuilder = Assembly.GetCallingAssembly().GetTypes().Where(t => t.BaseType == builder.GetType()).Select(Activator.CreateInstance).SingleOrDefault();

            if (customBuilder != null) return customBuilder as BaseBuilder<TSource, TDestination>;

            if (Mapper.FindTypeMapFor<TSource, TDestination>() == null) Mapper.AddProfile(builder);

            return builder;
        }
    }
}
