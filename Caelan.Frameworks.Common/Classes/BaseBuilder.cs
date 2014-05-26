using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Caelan.Frameworks.Common.Extenders;

namespace Caelan.Frameworks.Common.Classes
{
    public class BaseBuilder<TSource, TDestination> : Profile
    {
        sealed protected override void Configure()
        {
            base.Configure();

            var mappingExpression = Mapper.CreateMap<TSource, TDestination>();

            mappingExpression.AfterMap((source, destination) => AfterBuild(source, ref destination));

            AddMappingConfigurations(mappingExpression);
        }

        protected virtual void AddMappingConfigurations(IMappingExpression<TSource, TDestination> mappingExpression)
        {
            mappingExpression.IgnoreAllNonExisting();
        }

        public TDestination Build(TSource source)
        {
            if (source == null || source.Equals(default(TSource))) return default(TDestination);

            var dest = typeof(TDestination).IsValueType ? default(TDestination) : Activator.CreateInstance<TDestination>();

            Build(source, ref dest);

            return dest;
        }

        public IEnumerable<TDestination> BuildList(IEnumerable<TSource> sourceList)
        {
            return sourceList == null ? null : sourceList.Select(Build);
        }

        public void Build(TSource source, ref TDestination destination)
        {
            if (source == null || source.Equals(default(TSource)))
            {
                destination = default(TDestination);
                return;
            }

            Mapper.DynamicMap(source, destination);
        }

        public virtual void AfterBuild(TSource source, ref TDestination destination)
        {
        }
    }
}
