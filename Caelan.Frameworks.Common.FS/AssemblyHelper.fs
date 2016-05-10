namespace Caelan.Frameworks.Common.Helpers

open System
open System.Reflection
open System.Web

module AssemblyHelper =
    let GetWebEntryAssembly() =
        match HttpContext.Current with
        | currentContext when currentContext |> (isNull >> not) && currentContext.ApplicationInstance |> (isNull >> not) -> 
            let rec getAssembly (asmType : Type) =
                match asmType |> Option.ofObj with
                | None -> null
                | Some(value) when value.Namespace <> "ASP" -> value.Assembly
                | Some(value) -> value.BaseType |> getAssembly

            HttpContext.Current.ApplicationInstance.GetType() |> getAssembly
        | _ -> null