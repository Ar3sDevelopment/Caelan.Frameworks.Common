namespace Caelan.Frameworks.Common.Attributes
open System

type MapFieldAttribute(toField : string) = 
    inherit Attribute()
    member __.ToField = toField