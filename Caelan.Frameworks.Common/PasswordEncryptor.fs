namespace Caelan.Frameworks.Common.Classes

open Caelan.Frameworks.Common.Helpers
open Caelan.Frameworks.Common.Interfaces
open System
open System.Text
open System.IO
open System.Security.Cryptography

type PasswordEncryptor(defaultPassword : string, secret : string, salt : string, encryptor : IPasswordEncryptor) = 
    /// <summary>
    /// 
    /// </summary>
    member val DefaultPassword = defaultPassword
    /// <summary>
    /// 
    /// </summary>
    member this.DefaultPasswordEncrypted = this.EncryptPassword(this.DefaultPassword)
    /// <summary>
    /// This function encrypts given password using the encryptor inside the class
    /// </summary>
    /// <param name="password">The password to be encrypted</param>
    member __.EncryptPassword(password) = (encryptor, password) |> MemoizeHelper.Memoize(fun (e, p) -> e.EncryptPassword(p, secret, salt))
    /// <summary>
    /// This function decrypts given password using the encryptor inside the class
    /// </summary>
    /// <param name="crypted">The crypted data to be decrypted</param>
    member __.DecryptPassword(crypted) = (encryptor, crypted) |> MemoizeHelper.Memoize(fun (e, p) -> e.DecryptPassword(p, secret, salt))
    new(defaultPassword, secret, salt) = 
        let encryptor = 
            { new IPasswordEncryptor with
                  member __.EncryptPassword(password, secret, salt) = 
                      let saltBytes = salt |> Encoding.ASCII.GetBytes
                      using (new Rfc2898DeriveBytes(secret, saltBytes)) (fun key ->
                          using (new RijndaelManaged()) (fun aes ->
                              aes.Key <- key.GetBytes(aes.KeySize / 8)

                              let encryptor = aes.CreateEncryptor(aes.Key, aes.IV)

                              using (new MemoryStream()) (fun msEncrypt ->
                                  msEncrypt.Write(BitConverter.GetBytes(aes.IV.Length), 0, sizeof<int>)
                                  msEncrypt.Write(aes.IV, 0, aes.IV.Length)

                                  using (new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) (fun csEncrypt ->
                                      using (new StreamWriter(csEncrypt)) (fun swEncrypt ->
                                          swEncrypt.Write(password)
                                      )
                                  )

                                  Convert.ToBase64String(msEncrypt.ToArray())
                              )
                          )
                      )
                  
                  member __.DecryptPassword(crypted, secret, salt) = 
                      let saltBytes = salt |> Encoding.ASCII.GetBytes
                      using (new MemoryStream(Convert.FromBase64String(crypted))) (fun msDecrypt ->
                          let mutable rawLength : byte[] = sizeof<int> |> Array.zeroCreate 

                          msDecrypt.Read(rawLength, 0, rawLength.Length) |> ignore

                          let mutable buffer : byte[] = BitConverter.ToInt32(rawLength, 0) |> Array.zeroCreate

                          msDecrypt.Read(buffer, 0, buffer.Length) |> ignore

                          let iv = buffer
                          using (new Rfc2898DeriveBytes(secret, saltBytes)) (fun key ->
                              using (new RijndaelManaged()) (fun aes ->
                                  aes.Key <- key.GetBytes(aes.KeySize / 8)
                                  aes.IV <- iv

                                  let decryptor = aes.CreateDecryptor(aes.Key, aes.IV)

                                  using (new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) (fun csDecrypt ->
                                      using (new StreamReader(csDecrypt)) (fun srDecrypt ->
                                          srDecrypt.ReadToEnd()
                                      )
                                  )
                              )
                          )
                      ) }
        PasswordEncryptor(defaultPassword, secret, salt, encryptor)
