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
            return CreateGenericBuilder<BaseBuilder<TSource, TDestination>, TSource, TDestination>();
        }

        public static TBuilder CreateGenericBuilder<TBuilder, TSource, TDestination>()
            where TBuilder : BaseBuilder<TSource, TDestination>
            where TSource : class, new()
            where TDestination : class, new()
        {
            var builder = Activator.CreateInstance<TBuilder>();

            var customBuilder = Assembly.GetAssembly(typeof(TDestination)).GetTypes().SingleOrDefault(t => t.BaseType == builder.GetType()) ?? (Assembly.GetAssembly(typeof(TDestination))).GetReferencedAssemblies().OrderBy(t => t.Name).Select(Assembly.Load).SelectMany(assembly => assembly.GetTypes().Where(t => t.BaseType == builder.GetType())).SingleOrDefault();

            if (customBuilder != null) return Activator.CreateInstance(customBuilder) as TBuilder;

            customBuilder = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetTypes().SingleOrDefault(t => t.BaseType == builder.GetType()) ?? (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetReferencedAssemblies().OrderBy(t => t.Name).Select(Assembly.Load).SelectMany(assembly => assembly.GetTypes().Where(t => t.BaseType == builder.GetType())).SingleOrDefault();

            if (customBuilder != null) return Activator.CreateInstance(customBuilder) as TBuilder;

            if (Mapper.FindTypeMapFor<TSource, TDestination>() == null) Mapper.AddProfile(builder);

            return builder;
        }
    }
}
