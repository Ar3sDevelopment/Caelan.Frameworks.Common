namespace Caelan.Frameworks.PasswordHashing.Classes

open System.Security.Cryptography
open System.Text
open Caelan.Frameworks.PasswordHashing.Interfaces
open Caelan.Frameworks.Common.Helpers

type PasswordHasher(salt : string, defaultPassword : string, encryptor : IPasswordHasher) = 
    
    /// <summary>
    /// 
    /// </summary>
    member val Salt = salt
    
    /// <summary>
    /// 
    /// </summary>
    member val DefaultPassword = defaultPassword
    
    /// <summary>
    /// 
    /// </summary>
    member this.DefaultPasswordHashed = this.HashPassword(this.DefaultPassword)
    
    /// <summary>
    /// This function hashes the given password
    /// </summary>
    /// <param name="password">The password to be hashed</param>
    member this.HashPassword(password) = 
        (encryptor, password) |> MemoizeHelper.Memoize(fun (e, p) -> e.HashPassword(this.Salt + e.HashPassword(p)))
    
    new(salt, defaultPassword) = 
        let encryptor = 
            { new IPasswordHasher with
                  member __.HashPassword(password) = 
                      let computeHash (provider: SHA512CryptoServiceProvider) =
                          provider.ComputeHash(Encoding.Default.GetBytes(password))
                          |> Array.map (fun t -> t.ToString("x2").ToLower())
                          |> String.concat ""
                      using (new SHA512CryptoServiceProvider()) computeHash }
        PasswordHasher(salt, defaultPassword, encryptor)
