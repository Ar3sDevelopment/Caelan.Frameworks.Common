namespace Caelan.Frameworks.Common.Helpers

open System.Security.Cryptography
open System.Text

[<AbstractClass>]
type PasswordHelper() =
    abstract member GetSalt : unit -> string
    abstract member GetDefaultPassword : unit -> string

    abstract member GetDefaultPasswordEncrypted : unit -> string
    default this.GetDefaultPasswordEncrypted() =
        this.EncryptPassword(this.GetDefaultPassword())

    abstract member Sha512Encrypt : password:string -> string
    default this.Sha512Encrypt(password) =
        use provider = new SHA512CryptoServiceProvider()
        provider.ComputeHash(Encoding.Default.GetBytes(password)) |> Array.map(fun t -> t.ToString("x2").ToLower()) |> String.concat ""
        
    abstract member EncryptPassword : password:string -> string
    default this.EncryptPassword(password) =
        this.Sha512Encrypt(this.GetSalt() + password)