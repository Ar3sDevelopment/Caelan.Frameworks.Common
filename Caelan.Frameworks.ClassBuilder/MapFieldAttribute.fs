namespace Caelan.Frameworks.ClassBuilder.Attributes
open System

[<AllowNullLiteral>]
[<AttributeUsage(AttributeTargets.Field ||| AttributeTargets.Property)>]
type MapFieldAttribute(toField : string) = 
    inherit Attribute()
    member __.ToField = toField