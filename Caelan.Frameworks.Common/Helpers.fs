namespace Caelan.Frameworks.Common.Helpers

open System.Security.Cryptography
open System.Text

[<AbstractClass>]
type PasswordHelper() = 
    abstract GetSalt : unit -> string
    abstract GetDefaultPassword : unit -> string
    abstract GetDefaultPasswordEncrypted : unit -> string
    override this.GetDefaultPasswordEncrypted() = this.EncryptPassword(this.GetDefaultPassword())
    abstract Sha512Encrypt : password:string -> string
    
    override __.Sha512Encrypt(password) = 
        use provider = new SHA512CryptoServiceProvider()
        provider.ComputeHash(Encoding.Default.GetBytes(password))
        |> Array.map (fun t -> t.ToString("x2").ToLower())
        |> String.concat ""
    
    abstract EncryptPassword : password:string -> string
    override this.EncryptPassword(password) = this.Sha512Encrypt(this.GetSalt() + this.Sha512Encrypt(password))
