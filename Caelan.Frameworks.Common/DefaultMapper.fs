namespace Caelan.Frameworks.Common.Classes
open System
open Caelan.Frameworks.Common.Attributes
open Caelan.Frameworks.Common.Interfaces

type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>(source) = 
    let mutable mutableSource = source
    interface IMapper<'TSource, 'TDestination> with
        member __.Source
            with set(v) = mutableSource <- v
        member this.Map() = this.Map()
        member this.Map (destination: 'TDestination) = this.Map(destination)

    abstract Map : destination:'TDestination -> unit
    abstract Map : unit -> 'TDestination

    override this.Map (destination : 'TDestination) =
        let d = ref destination
        let sourceType = mutableSource.GetType()
        let customProperties =
            sourceType.GetProperties()
            |> Array.filter (fun t -> t.CustomAttributes |> Seq.length > 0)
        match sourceType with
        | _ when sourceType.GetCustomAttributes(typeof<MapEqualsAttribute>, true) |> Array.length > 0 ->
            sourceType.GetProperties()
            |> Array.filter (fun t ->  d.Value.GetType().GetProperty(t.Name) <> null)
            |> Array.Parallel.iter (fun t ->
                match d.Value.GetType().GetProperty(t.Name) with
                | null -> ()
                | property -> property.SetValue(d.Value, t.GetValue(mutableSource)))
        | _ -> 
            customProperties
            |> Array.Parallel.map (fun t ->
                (t, match Attribute.GetCustomAttribute(t,typeof<MapEqualsAttribute>) with
                    | null -> null
                    | attribute -> attribute :?> MapEqualsAttribute))
            |> Array.Parallel.iter (fun (t, a) ->
                match a with
                | null -> ()
                | _ -> d.Value.GetType().GetProperty(t.Name).SetValue(d.Value, t.GetValue(mutableSource)))

        customProperties
        |> Array.Parallel.map (fun t ->
            (t, match Attribute.GetCustomAttribute(t,typeof<MapFieldAttribute>) with
                | null -> null
                | attribute -> attribute :?> MapFieldAttribute))
        |> Array.Parallel.iter (fun (t, a) ->
            match a with
            | null -> ()
            | _ ->
                match d.Value.GetType().GetProperty(a.ToField) with
                | null -> ()
                | property -> property.SetValue(d.Value, t.GetValue(mutableSource)))

    override this.Map() = 
        match mutableSource with
        | null -> Unchecked.defaultof<'TDestination>
        | _ -> 
            let dest = Activator.CreateInstance<'TDestination>()
            this.Map(dest)
            dest
