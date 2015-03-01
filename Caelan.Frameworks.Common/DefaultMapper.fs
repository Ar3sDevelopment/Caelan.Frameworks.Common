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
            |> Array.filter (fun t ->  destination.GetType().GetProperty(t.Name) <> null)
            |> Array.Parallel.iter (fun t ->
                match destination.GetType().GetProperty(t.Name) with
                | null -> ()
                | property -> property.SetValue(destination, t.GetValue(source)))
        | _ -> 
            customProperties
            |> Array.Parallel.map (fun t ->
                (t, match Attribute.GetCustomAttribute(t,typeof<MapEqualsAttribute>) with
                    | null -> null
                    | attribute -> attribute :?> MapEqualsAttribute))
            |> Array.Parallel.iter (fun (t, a) ->
                match a with
                | null -> ()
                | _ -> destination.GetType().GetProperty(t.Name).SetValue(destination, t.GetValue(source)))

        customProperties
        |> Array.Parallel.map (fun t ->
            (t, match Attribute.GetCustomAttribute(t,typeof<MapFieldAttribute>) with
                | null -> null
                | attribute -> attribute :?> MapFieldAttribute))
        |> Array.Parallel.iter (fun (t, a) ->
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
