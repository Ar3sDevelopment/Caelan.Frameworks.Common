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

            var customBuilder = Assembly.GetExecutingAssembly().GetReferencedAssemblies().OrderBy(t => t.Name).Select(Assembly.Load).SelectMany(assembly => assembly.GetTypes().Where(t => t.BaseType == builder.GetType())).SingleOrDefault();

            if (customBuilder != null) return Activator.CreateInstance(customBuilder) as BaseBuilder<TSource, TDestination>;

            if (Mapper.FindTypeMapFor<TSource, TDestination>() == null) Mapper.AddProfile(builder);

            return builder;
        }
    }
}
