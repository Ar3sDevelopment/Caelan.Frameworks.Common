namespace Caelan.Frameworks.Common.Extenders

open AutoMapper
open AutoMapper.Internal
open System.Reflection
open System.Runtime.CompilerServices

module AutoMapperExtender =
        [<Extension>]
        let IgnoreAllNonExisting(expression : IMappingExpression<'TSource, 'TDestination>) =
            let typeMaps = Mapper.GetAllTypeMaps()
            let item = Array.find (fun (item : TypeMap) -> item.SourceType.Equals(typedefof<'TSource>) && item.DestinationType.Equals(typedefof<'TDestination>)) typeMaps
            for prop in item.GetUnmappedPropertyNames() do
                expression.ForMember(prop, fun opt -> opt.Ignore()) |> ignore

        [<Extension>]
        let IgnoreAllNonPrimitive (expression : IMappingExpression<'TSource, 'TDestination>) =
            let properties = (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
            let filteredProperties = Array.filter (fun (item : PropertyInfo) -> item.PropertyType.IsPrimitive.Equals(false) && item.PropertyType.IsValueType.Equals(false) && item.PropertyType.Equals(typedefof<string>).Equals(false)) properties
            for prop in filteredProperties do
                expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore

        [<Extension>]
        let IgnoreAllLists (expression : IMappingExpression<'TSource, 'TDestination>) =
            let properties = (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
            let filteredProperties = Array.filter (fun (item : PropertyInfo) -> item.PropertyType.IsEnumerableType().Equals(true) && item.PropertyType.Equals(typedefof<string>).Equals(false)) properties
            for prop in filteredProperties do
                expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore