namespace Caelan.Frameworks.Common.Extenders

open AutoMapper
open AutoMapper.Internal
open System.Reflection
open System.Runtime.CompilerServices

[<Extension>]
type public AutoMapperExtender = 
    
    [<Extension>]
    static member IgnoreAllNonExisting(expression : IMappingExpression<'TSource, 'TDestination>) = 
        match Mapper.GetAllTypeMaps() 
              |> Seq.tryFind 
                     (fun (item : TypeMap) -> 
                     item.SourceType.Equals(typeof<'TSource>) && item.DestinationType.Equals(typeof<'TDestination>)) with
        | Some(i) -> 
            i.GetUnmappedPropertyNames() 
            |> Seq.iter (fun prop -> expression.ForMember(prop, fun opt -> opt.Ignore()) |> ignore)
        | None -> ()
    
    [<Extension>]
    static member IgnoreAllNonPrimitive(expression : IMappingExpression<'TSource, 'TDestination>) = 
        (typeof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        |> Seq.filter 
               (fun item -> 
               (item.PropertyType.IsPrimitive || item.PropertyType.IsValueType || item.PropertyType = typeof<string>) = false)
        |> Seq.iter (fun prop -> expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore)
    
    [<Extension>]
    static member IgnoreAllLists(expression : IMappingExpression<'TSource, 'TDestination>) = 
        (typeof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        |> Seq.filter (fun item -> item.PropertyType.IsEnumerableType() && item.PropertyType <> typeof<string>)
        |> Seq.iter (fun prop -> expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore)