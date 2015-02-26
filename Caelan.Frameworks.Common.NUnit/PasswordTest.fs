namespace Caelan.Frameworks.Common.NUnit
open System
open NUnit.Framework
open Caelan.Frameworks.Common.Classes

type CustomPasswordHasher() =
    inherit PasswordHasher("salt", "default")

[<TestFixture>]
type Test() = 

    [<Test>]
    member __.TestPassword() =
        let pwd = CustomPasswordHasher()

        "password" |> pwd.HashPassword |> printfn "%s"

