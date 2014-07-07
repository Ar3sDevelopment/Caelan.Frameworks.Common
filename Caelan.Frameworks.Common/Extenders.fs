namespace Caelan.Frameworks.Common.Extenders

open AutoMapper
open AutoMapper.Internal
open System.Reflection
open System.Runtime.CompilerServices

module AutoMapperExtender = 
    [<Extension>]
    let IgnoreAllNonExisting(expression : IMappingExpression<'TSource, 'TDestination>) = 
        let typeMaps = Mapper.GetAllTypeMaps()
        let item = 
            Array.find 
                (fun (item : TypeMap) -> 
                item.SourceType.Equals(typedefof<'TSource>) && item.DestinationType.Equals(typedefof<'TDestination>)) 
                typeMaps
        for prop in item.GetUnmappedPropertyNames() do
            expression.ForMember(prop, fun opt -> opt.Ignore()) |> ignore
    
    [<Extension>]
    let IgnoreAllNonPrimitive(expression : IMappingExpression<'TSource, 'TDestination>) = 
        let properties = (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        let filteredProperties = 
            properties 
            |> Array.filter 
                   (fun item -> 
                   (item.PropertyType.IsPrimitive || item.PropertyType.IsValueType 
                    || item.PropertyType = typedefof<string>) = false)
        for prop in filteredProperties do
            expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore
    
    [<Extension>]
    let IgnoreAllLists(expression : IMappingExpression<'TSource, 'TDestination>) = 
        let properties = (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        let filteredProperties = 
            properties 
            |> Array.filter (fun item -> item.PropertyType.IsEnumerableType() && item.PropertyType <> typedefof<string>)
        for prop in filteredProperties do
            expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore
