namespace Caelan.Frameworks.Common.Classes
open System
open Caelan.Frameworks.Common.Attributes
open Caelan.Frameworks.Common.Interfaces

type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source) = this.Map(source)
        member this.Map (source, destination) = this.Map(source,destination)

    abstract Map : source:'TSource * destination:'TDestination -> unit
    abstract Map : source:'TSource -> 'TDestination

    override __.Map (source, destination) =
        let sourceType = source.GetType()
        let customProperties =
            sourceType.GetProperties()
            |> Array.filter (fun t -> t.CustomAttributes |> Seq.length > 0)
        match sourceType with
        | _ when sourceType.GetCustomAttributes(typeof<MapEqualsAttribute>, true) |> Array.length > 0 ->
            sourceType.GetProperties()
            |> Array.choose (fun t -> match destination.GetType().GetProperty(t.Name) with
                                      | null -> None
                                      | property -> Some (property, t.GetValue(source)))
            |> Array.iter (fun (property, value) -> property.SetValue(destination, value))
        | _ -> 
            customProperties
            |> Array.filter (fun t -> Attribute.GetCustomAttribute(t,typeof<MapEqualsAttribute>) <> null)
            |> Array.iter (fun t ->
                match destination.GetType().GetProperty(t.Name) with
                | null -> ()
                | property -> property.SetValue(destination, t.GetValue(source)))

        customProperties
        |> Array.map (fun t -> (t, Attribute.GetCustomAttribute(t,typeof<MapFieldAttribute>) :?> MapFieldAttribute))
        |> Array.iter (fun (t, a) ->
            match a with
            | null -> ()
            | _ ->
                match destination.GetType().GetProperty(a.ToField) with
                | null -> ()
                | property -> property.SetValue(destination, t.GetValue(source)))

    override this.Map(source) = 
        match source with
        | null -> Unchecked.defaultof<'TDestination>
        | _ -> 
            let dest = Activator.CreateInstance<'TDestination>()
            this.Map(source, dest)
            dest
