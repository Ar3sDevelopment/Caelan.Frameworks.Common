namespace Caelan.Frameworks.Common.Interfaces

open System

type IPasswordEncryptor = 
    abstract EncryptPassword : password:string -> string