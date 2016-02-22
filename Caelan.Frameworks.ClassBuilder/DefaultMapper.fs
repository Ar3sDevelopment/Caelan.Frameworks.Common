namespace Caelan.Frameworks.ClassBuilder.Classes
open System
open System.Reflection
open Caelan.Frameworks.ClassBuilder.Attributes
open Caelan.Frameworks.ClassBuilder.Interfaces

type DefaultMapper<'TSource, 'TDestination>() = 
    interface IMapper<'TSource, 'TDestination> with
        member this.Map(source) = (this :> IMapper<'TSource, 'TDestination>).Map(source, Activator.CreateInstance<'TDestination>())
        member this.Map(source, destination) =
            let iterGeneric name value =
                match destination.GetType().GetProperty(name) |> Option.ofObj with
                | None -> ()
                | Some(property) -> property.SetValue(destination, value)
            let iterEqualsProperties (p : PropertyInfo) = (p.Name, p.GetValue(source)) ||> iterGeneric
            let iterMapProperties (p : PropertyInfo, attribute : MapFieldAttribute) =
                match attribute |> Option.ofObj with
                | None -> ()
                | Some(a) -> (a.ToField, p.GetValue(source)) ||> iterGeneric
            match source :> obj |> Option.ofObj with
            | None -> Unchecked.defaultof<'TDestination>
            | _ ->
                let sourceType = source.GetType()
                let customProperties = sourceType.GetProperties() |> Array.filter (fun t -> t.CustomAttributes |> Seq.length > 0)
                match sourceType with
                | st when st.GetCustomAttributes(typeof<MapEqualsAttribute>, true) |> Array.length > 0 ->
                    st.GetProperties() |> Array.iter iterEqualsProperties
                | _ -> 
                    customProperties
                    |> Array.filter (fun t -> Attribute.GetCustomAttribute(t,typeof<MapEqualsAttribute>) |> (isNull >> not))
                    |> Array.iter iterEqualsProperties

                customProperties
                |> Array.map (fun t -> (t, Attribute.GetCustomAttribute(t,typeof<MapFieldAttribute>) :?> MapFieldAttribute))
                |> Array.iter iterMapProperties
                (source, destination) |> this.CustomMap
                

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    abstract CustomMap : source:'TSource * destination:'TDestination -> 'TDestination
    override __.CustomMap(source, destination) = destination