namespace Caelan.Frameworks.ClassBuilder.Attributes
open System

[<AllowNullLiteral>]
[<AttributeUsage(AttributeTargets.Field ||| AttributeTargets.Property ||| AttributeTargets.Class)>]
type MapEqualsAttribute() = 
    inherit Attribute()