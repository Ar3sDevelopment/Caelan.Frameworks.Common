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
                     item.SourceType.Equals(typeof<'TSource>) 
                     && item.DestinationType.Equals(typeof<'TDestination>)) with
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

[<Extension>]
type public FunctionConverter = 
    
    [<Extension>]
    static member ToFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TOut>>) = <@ fun _ -> expr.Compile().Invoke() @>

    [<Extension>]
    static member ToFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TIn, 'TOut>>) = <@ fun p -> expr.Compile().Invoke(p) @>

    [<Extension>]
    static member ToFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TIn1, 'TIn2, 'TOut>>) = <@ fun p1 p2 -> expr.Compile().Invoke(p1, p2) @>

    [<Extension>]
    static member ToFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>>) = <@ fun p1 p2 p3 -> expr.Compile().Invoke(p1, p2, p3) @>

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

    [<Extension>]
    static member ToSystemExpr(expr : unit -> 'TOut) = <@ System.Func<'TOut>(expr) @>
    
    [<Extension>]
    static member ToSystemExpr(expr : 'TIn -> 'TOut) = <@ System.Func<'TIn, 'TOut>(expr) @>
    
    [<Extension>]
    static member ToSystemExpr(expr : 'TIn1 -> 'TIn2 -> 'TOut) = <@ System.Func<'TIn1, 'TIn2, 'TOut>(expr) @>
    
    [<Extension>]
    static member ToSystemExpr(expr : 'TIn1 -> 'TIn2 -> 'TIn3 -> 'TOut) = <@ System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>(expr) @>

    static member CreateFSharpFunc(expr : System.Func<'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateFSharpFunc(expr : System.Func<'TIn, 'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateFSharpFunc(expr : System.Func<'TIn1, 'TIn2, 'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateFSharpFunc(expr : System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>) = FunctionConverter.ToFSharpFunc expr

    static member CreateSystemFunc(expr : unit -> 'TOut) = System.Func<'TOut> expr

    static member CreateSystemFunc(expr : 'TIn -> 'TOut) = System.Func<'TIn, 'TOut> expr

    static member CreateSystemFunc(expr : 'TIn1 -> 'TIn2 -> 'TOut) = System.Func<'TIn1, 'TIn2, 'TOut> expr

    static member CreateSystemFunc(expr : 'TIn1 -> 'TIn2 -> 'TIn3 -> 'TOut) = System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>(expr)

    static member CreateFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TOut>>) = FunctionConverter.ToFSharpExpr expr

    static member CreateFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TIn, 'TOut>>) = FunctionConverter.ToFSharpExpr expr

    static member CreateFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TIn1, 'TIn2, 'TOut>>) = FunctionConverter.ToFSharpExpr expr

    static member CreateFSharpExpr(expr : System.Linq.Expressions.Expression<System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>>) = FunctionConverter.ToFSharpExpr expr

    static member CreateSystemExpr(expr : unit -> 'TOut) = <@ System.Func<'TOut> expr @>

    static member CreateSystemExpr(expr : 'TIn -> 'TOut) = <@ System.Func<'TIn, 'TOut> expr @>

    static member CreateSystemExpr(expr : 'TIn1 -> 'TIn2 -> 'TOut) = <@ System.Func<'TIn1, 'TIn2, 'TOut> expr @>

    static member CreateSystemExpr(expr : 'TIn1 -> 'TIn2 -> 'TIn3 -> 'TOut) = <@ System.Func<'TIn1, 'TIn2, 'TIn3, 'TOut>(expr) @>
