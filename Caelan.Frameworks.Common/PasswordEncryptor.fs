namespace Caelan.Frameworks.Common.Classes

open Caelan.Frameworks.Common.Helpers
open Caelan.Frameworks.Common.Interfaces
open System
open System.Text

type PasswordEncryptor(defaultPassword : string, encryptor : IPasswordEncryptor) = 
    member val DefaultPassword = defaultPassword
    member this.DefaultPasswordEncrypted = this.EncryptPassword(this.DefaultPassword)
    member __.EncryptPassword(password) = (encryptor, password) |> MemoizeHelper.Memoize(fun (e, p) -> e.EncryptPassword(p))
    member __.DecryptPassword(crypted) = (encryptor, crypted) |> MemoizeHelper.Memoize(fun (e, p) -> e.DecryptPassword(p))
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
