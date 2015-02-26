namespace Caelan.Frameworks.Common.Classes

open System.Security.Cryptography
open System.Text
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

type PasswordHasher(salt : string, defaultPassword : string, encryptor : IPasswordHasher) = 
    member val Salt = salt with get
    member val DefaultPassword = defaultPassword with get
    member this.DefaultPasswordEncrypted with get() = this.HashPassword(this.DefaultPassword)
    member this.HashPassword(password) = 
        (encryptor, password) 
        |> MemoizeHelper.Memoize(fun (e, p) -> e.HashPassword(this.Salt + e.HashPassword(p)))
    new(salt, defaultPassword) = 
        let encryptor = 
            { new IPasswordHasher with
                  member __.HashPassword(password) = 
                      using (new SHA512CryptoServiceProvider()) (fun provider -> 
                          provider.ComputeHash(Encoding.Default.GetBytes(password))
                          |> Array.map (fun t -> t.ToString("x2").ToLower())
                          |> String.concat "") }
        PasswordHasher(salt, defaultPassword, encryptor)
