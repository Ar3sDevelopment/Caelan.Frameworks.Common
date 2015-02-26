namespace Caelan.Frameworks.Common.Classes

open System
open System.Security.Cryptography
open System.Text
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Helpers

type PasswordEncryptor(defaultPassword : string, encryptor : IPasswordEncryptor) = 
    member val DefaultPassword = defaultPassword with get
    member this.DefaultPasswordEncrypted with get() = this.EncryptPassword(this.DefaultPassword)
    member this.EncryptPassword(password) = 
        (encryptor, password) 
        |> MemoizeHelper.Memoize(fun (e, p) -> e.DecryptPassword(p))
    member this.DecryptPassword(crypted) = 
        (encryptor, crypted) 
        |> MemoizeHelper.Memoize(fun (e, p) -> e.DecryptPassword(p))
    new(defaultPassword) = 
        let encryptor = 
            { new IPasswordEncryptor with
                  member __.EncryptPassword(password) = 
                      let bytes = Encoding.UTF8.GetBytes(password)
                      Convert.ToBase64String(bytes)
                  member __.DecryptPassword(crypted) = 
                      let bytes = Convert.FromBase64String(crypted)
                      Encoding.UTF8.GetString(bytes) }
        PasswordEncryptor(defaultPassword, encryptor)
    