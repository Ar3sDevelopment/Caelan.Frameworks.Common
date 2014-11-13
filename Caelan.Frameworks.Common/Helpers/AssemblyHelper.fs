namespace Caelan.Frameworks.Common.Helpers

open System
open System.Reflection
open System.Web

module AssemblyHelper =
    let GetWebEntryAssembly() =
        if HttpContext.Current = null || HttpContext.Current.ApplicationInstance = null then
            null
        else
            let mutable t = HttpContext.Current.ApplicationInstance.GetType()
            while t <> null && t.Namespace = "ASP" do
                t <- t.BaseType
            
            match t with
            | null -> null
            | _ -> t.Assembly