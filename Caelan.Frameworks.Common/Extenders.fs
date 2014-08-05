namespace Caelan.Frameworks.Common.Extenders

open AutoMapper
open AutoMapper.Internal
open System.Reflection
open System.Runtime.CompilerServices

module AutoMapperExtender = 
    [<Extension>]
    let IgnoreAllNonExisting(expression : IMappingExpression<'TSource, 'TDestination>) = 
        match Mapper.GetAllTypeMaps() 
              |> Seq.tryFind 
                     (fun (item : TypeMap) -> 
                     item.SourceType.Equals(typedefof<'TSource>) 
                     && item.DestinationType.Equals(typedefof<'TDestination>)) with
        | Some(i) -> 
            i.GetUnmappedPropertyNames() 
            |> Seq.iter (fun prop -> expression.ForMember(prop, fun opt -> opt.Ignore()) |> ignore)
        | None -> ()
    
    [<Extension>]
    let IgnoreAllNonPrimitive(expression : IMappingExpression<'TSource, 'TDestination>) = 
        (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        |> Seq.filter 
               (fun item -> 
               (item.PropertyType.IsPrimitive || item.PropertyType.IsValueType || item.PropertyType = typedefof<string>) = false)
        |> Seq.iter (fun prop -> expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore)
    
    [<Extension>]
    let IgnoreAllLists(expression : IMappingExpression<'TSource, 'TDestination>) = 
        (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        |> Seq.filter (fun item -> item.PropertyType.IsEnumerableType() && item.PropertyType <> typedefof<string>)
        |> Seq.iter (fun prop -> expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore)
