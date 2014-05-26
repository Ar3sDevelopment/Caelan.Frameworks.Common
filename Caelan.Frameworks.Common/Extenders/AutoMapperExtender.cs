using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Internal;

namespace Caelan.Frameworks.Common.Extenders
{
	public static class AutomapperExtender
	{
		public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
		{
			foreach (var property in Mapper.GetAllTypeMaps().First(x => x.SourceType == typeof(TSource) && x.DestinationType == typeof(TDestination)).GetUnmappedPropertyNames())
			{
				expression.ForMember(property, opt => opt.Ignore());
			}

			return expression;
		}

		public static IMappingExpression<TSource, TDestination> IgnoreAllNonPrimitive<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
		{
			foreach (var prop in from prop in typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance) let propType = prop.PropertyType where !(propType.IsPrimitive || propType.IsValueType || propType == typeof(string)) select prop)
			{
				expression.ForMember(prop.Name, o => o.Ignore());
			}

			return expression;
		}

		public static IMappingExpression<TSource, TDestination> IgnoreAllLists<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
		{
			foreach (var prop in from prop in typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance) let propType = prop.PropertyType where propType.IsEnumerableType() && propType != typeof(string) select prop)
			{
				expression.ForMember(prop.Name, o => o.Ignore());
			}

			return expression;
		}
	}
}
