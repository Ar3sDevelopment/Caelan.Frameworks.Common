namespace Caelan.Frameworks.Common.Classes
open System
open Caelan.Frameworks.Common.Attributes
open Caelan.Frameworks.Common.Interfaces

type DefaultMapper<'TSource, 'TDestination>() = 
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source) =
            let destination = Activator.CreateInstance<'TDestination>()
            (this :> IMapper<'TSource, 'TDestination>).Map(source, destination)
        member this.Map(source, destination) =
            let sourceType = source.GetType()
            let customProperties = sourceType.GetProperties() |> Array.filter (fun t -> t.CustomAttributes |> Seq.length > 0)
            match sourceType with
            | _ when sourceType.GetCustomAttributes(typeof<MapEqualsAttribute>, true) |> Array.length > 0 ->
                sourceType.GetProperties()
                |> Array.choose (fun t -> match destination.GetType().GetProperty(t.Name) |> Option.ofObj with
                                          | None -> None
                                          | Some(property) -> Some (property, t.GetValue(source)))
                |> Array.iter (fun (property, value) -> property.SetValue(destination, value))
            | _ -> 
                customProperties
                |> Array.filter (fun t -> Attribute.GetCustomAttribute(t,typeof<MapEqualsAttribute>) |> (isNull >> not))
                |> Array.iter (fun t ->
                    match destination.GetType().GetProperty(t.Name) |> Option.ofObj with
                    | None -> ()
                    | Some(property) -> property.SetValue(destination, t.GetValue(source)))

            customProperties
            |> Array.map (fun t -> (t, Attribute.GetCustomAttribute(t,typeof<MapFieldAttribute>) :?> MapFieldAttribute))
            |> Array.iter (fun (t, a) ->
                match a |> Option.ofObj with
                | None -> ()
                | Some(a) ->
                    match destination.GetType().GetProperty(a.ToField) |> Option.ofObj with
                    | None -> ()
                    | Some(property) -> property.SetValue(destination, t.GetValue(source)))
            match source :> obj |> Option.ofObj with
            | None -> Unchecked.defaultof<'TDestination>
            | Some(s) -> this.CustomMap(s :?> 'TSource, destination)

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    abstract CustomMap : source:'TSource * destination:'TDestination -> 'TDestination
    override __.CustomMap(source, destination) =
        destination