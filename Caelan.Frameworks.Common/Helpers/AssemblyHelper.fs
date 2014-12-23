namespace Caelan.Frameworks.Common.Helpers

open System
open System.Reflection
open System.Web

module AssemblyHelper =
    let GetWebEntryAssembly() =
        match HttpContext.Current with
        | currentContext when currentContext = null || currentContext.ApplicationInstance = null -> null
        | _ ->
            let rec getAssembly (asmType : Type) =
                match asmType with
                | null -> null
                | value when value.Namespace <> "ASP" -> value.Assembly
                | _ -> getAssembly asmType.BaseType

            HttpContext.Current.ApplicationInstance.GetType() |> getAssembly