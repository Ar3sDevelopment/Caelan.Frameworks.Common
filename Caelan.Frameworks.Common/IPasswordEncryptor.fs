namespace Caelan.Frameworks.Common.Interfaces

open System

type IPasswordEncryptor = 
    abstract EncryptPassword : password:string*secret:string*salt:string -> string
    abstract DecryptPassword : crypted:string*secret:string*salt:string -> string