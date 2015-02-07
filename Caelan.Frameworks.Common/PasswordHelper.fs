namespace Caelan.Frameworks.Common.Helpers

open System.Security.Cryptography
open System.Text
open Caelan.Frameworks.Common.Interfaces

type PasswordHelper(salt : string, defaultPassword : string, encryptor : IPasswordEncryptor) = 
    member val Salt = salt with get
    member val DefaultPassword = defaultPassword with get
    member this.DefaultPasswordEncrypted with get() = this.EncryptPassword(this.DefaultPassword)
    member this.EncryptPassword(password) = 
        (encryptor, password) 
        |> MemoizeHelper.Memoize(fun (e, p) -> e.EncryptPassword(this.Salt + e.EncryptPassword(p)))
    new(salt, defaultPassword) = 
        let encryptor = 
            { new IPasswordEncryptor with
                  member __.EncryptPassword(password) = 
                      using (new SHA512CryptoServiceProvider()) (fun provider -> 
                          provider.ComputeHash(Encoding.Default.GetBytes(password))
                          |> Array.map (fun t -> t.ToString("x2").ToLower())
                          |> String.concat "") }
        PasswordHelper(salt, defaultPassword, encryptor)
