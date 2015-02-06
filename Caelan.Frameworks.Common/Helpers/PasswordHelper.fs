namespace Caelan.Frameworks.Common.Helpers

open System.Security.Cryptography
open System.Text
open Caelan.Frameworks.Common.Interfaces

type PasswordHelper(salt : string, defaultPassword : string, encryptor : IPasswordEncryptor) = 
    member this.GetSalt() = salt
    member this.GetDefaultPassword() = defaultPassword
    member this.GetDefaultPasswordEncrypted() = this.EncryptPassword(this.GetDefaultPassword())
    member this.EncryptPassword(password) = 
        password |> MemoizeHelper.Memoize(fun p -> encryptor.EncryptPassword(this.GetSalt() + encryptor.EncryptPassword(p)))
    new(salt, defaultPassword) = 
        let encryptor = 
            { new IPasswordEncryptor with
                  member __.EncryptPassword(password) = 
                      using (new SHA512CryptoServiceProvider()) (fun provider -> 
                          provider.ComputeHash(Encoding.Default.GetBytes(password))
                          |> Array.map (fun t -> t.ToString("x2").ToLower())
                          |> String.concat "") }
        PasswordHelper(salt, defaultPassword, encryptor)
