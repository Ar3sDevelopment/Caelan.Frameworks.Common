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
                     item.SourceType.Equals(typedefof<'TSource>) 
                     && item.DestinationType.Equals(typedefof<'TDestination>)) with
        | Some(i) -> 
            i.GetUnmappedPropertyNames() 
            |> Seq.iter (fun prop -> expression.ForMember(prop, fun opt -> opt.Ignore()) |> ignore)
        | None -> ()
    
    [<Extension>]
    static member IgnoreAllNonPrimitive(expression : IMappingExpression<'TSource, 'TDestination>) = 
        (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        |> Seq.filter 
               (fun item -> 
               (item.PropertyType.IsPrimitive || item.PropertyType.IsValueType || item.PropertyType = typedefof<string>) = false)
        |> Seq.iter (fun prop -> expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore)
    
    [<Extension>]
    static member IgnoreAllLists(expression : IMappingExpression<'TSource, 'TDestination>) = 
        (typedefof<'TDestination>).GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
        |> Seq.filter (fun item -> item.PropertyType.IsEnumerableType() && item.PropertyType <> typedefof<string>)
        |> Seq.iter (fun prop -> expression.ForMember(prop.Name, fun opt -> opt.Ignore()) |> ignore)

[<Extension>]
type public FunctionConverter = 
    
    [<Extension>]
    static member ToFSharpFunc(expr : System.Func<'TOut>) = fun _ -> expr.Invoke()
    
    [<Extension>]
    static member ToFSharpFunc(expr : System.Func<'TIn, 'TOut>) = fun p -> expr.Invoke(p)
    
    [<Extension>]
    static member ToFSharpFunc(expr : System.Func<'TIn1, 'TIn2, 'TOut>) = fun p1 p2 -> expr.Invoke(p1, p2)
    
    [<Extension>]
    static member ToFSharpFunc(expr : System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>) = fun p1 p2 p3 -> expr.Invoke(p1, p2, p3)
    
    [<Extension>]
    static member ToSystemFunc(expr : unit -> 'TOut) = System.Func<'TOut>(expr)
    
    [<Extension>]
    static member ToSystemFunc(expr : 'TIn -> 'TOut) = System.Func<'TIn, 'TOut>(expr)
    
    [<Extension>]
    static member ToSystemFunc(expr : 'TIn1 -> 'TIn2 -> 'TOut) = System.Func<'TIn1, 'TIn2, 'TOut>(expr)
    
    [<Extension>]
    static member ToSystemFunc(expr : 'TIn1 -> 'TIn2 -> 'TIn3 -> 'TOut) = System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>(expr)

    static member CreateFSharpFunc(expr : System.Func<'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateFSharpFunc(expr : System.Func<'TIn, 'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateFSharpFunc(expr : System.Func<'TIn1, 'TIn2, 'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateFSharpFunc(expr : System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateSystemFunc(expr : unit -> 'TOut) = System.Func<'TOut> expr

    static member CreateSystemFunc(expr : 'TIn -> 'TOut) = System.Func<'TIn, 'TOut>(expr)

    static member CreateSystemFunc(expr : 'TIn1 -> 'TIn2 -> 'TOut) = System.Func<'TIn1, 'TIn2, 'TOut>(expr)

    static member CreateSystemFunc(expr : 'TIn1 -> 'TIn2 -> 'TIn3 -> 'TOut) = System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>(expr)
