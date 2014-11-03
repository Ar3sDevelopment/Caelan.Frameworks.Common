namespace Caelan.Frameworks.Common.Interfaces

open System

[<Interface>]
type IPasswordEncryptor = 
    abstract EncryptPassword : password:string -> string