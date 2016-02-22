namespace Caelan.Frameworks.Common.NUnit
open System
open NUnit.Framework
open Caelan.Frameworks.Common.Classes
open Caelan.Frameworks.PasswordEncryption.Classes
open Caelan.Frameworks.PasswordHashing.Classes

type CustomPasswordHasher() =
    inherit PasswordHasher("salt", "default")

type CustomPasswordEncryptor() =
    inherit PasswordEncryptor("default", "secret", "saltsalt")

[<TestFixture>]
type PasswordTest() = 

    [<Test>]
    member __.TestHasher() =
        let pwd = CustomPasswordHasher()

        "password" |> pwd.HashPassword |> printfn "%s"
        pwd.DefaultPasswordHashed |> printfn "%s"

    [<Test>]
    member __.TestEncryption() =
        let pwd = CustomPasswordEncryptor()

        let crypted = "password" |> pwd.EncryptPassword

        crypted |> printfn "%s"

        let decrypted = crypted |> pwd.DecryptPassword

        (decrypted, "password") |> Assert.AreEqual

        decrypted |> printfn "%s"

        pwd.DefaultPasswordEncrypted |> printfn "%s"
