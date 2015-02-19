namespace Caelan.Frameworks.Common.Classes
open System
open Caelan.Frameworks.Common.Attributes
open Caelan.Frameworks.Common.Interfaces

type DefaultMapper<'TSource, 'TDestination when 'TSource : equality and 'TSource : null and 'TSource : not struct and 'TDestination : equality and 'TDestination : null and 'TDestination : not struct>() = 
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source, destination: 'TDestination byref) = this.Map(source, ref destination)
        member this.Map(source) = this.Map(source)
        member this.Map (source, destination: 'TDestination) = this.Map(source, destination)

    abstract Map : source:'TSource * destination:'TDestination byref -> unit
    abstract Map : source:'TSource * destination:'TDestination -> 'TDestination
    abstract Map : source:'TSource -> 'TDestination

    override this.Map (source : 'TSource, destination : 'TDestination byref) =
        let d = ref destination
        match source.GetType() with
        | sourceType when sourceType.GetCustomAttributes(typeof<MapEqualsAttribute>, true) |> Array.length > 0 ->
            sourceType.GetProperties()
            |> Array.filter (fun t ->  d.Value.GetType().GetProperty(t.Name) <> null)
            |> Array.Parallel.iter (fun t -> d.Value.GetType().GetProperty(t.Name).SetValue(d.Value, t.GetValue(source)))
        | sourceType -> 
            let customProperties =
                sourceType.GetProperties()
                |> Array.filter (fun t -> t.CustomAttributes |> Seq.length > 0)
            customProperties
            |> Array.Parallel.map (fun t ->
                (t, match Attribute.GetCustomAttribute(t,typeof<MapEqualsAttribute>) with
                    | null -> null
                    | attribute -> attribute :?> MapEqualsAttribute))
            |> Array.Parallel.iter (fun (t, a) ->
                match a with
                | null -> ()
                | _ -> d.Value.GetType().GetProperty(t.Name).SetValue(d.Value, t.GetValue(source)))
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
                    | property -> property.SetValue(d.Value, t.GetValue(source)))

    override this.Map (source, destination : 'TDestination) =
        let newDest = destination
        this.Map(source, ref destination)
        destination
    override this.Map source = 
        match source with
        | null -> Unchecked.defaultof<'TDestination>
        | _ -> 
            this.Map(source, Activator.CreateInstance<'TDestination>())
