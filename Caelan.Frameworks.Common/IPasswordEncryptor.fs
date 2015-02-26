namespace Caelan.Frameworks.Common.Interfaces

type IPasswordEncryptor = 
    abstract EncryptPassword : password:string -> string
    abstract DecryptPassword : crypted:string -> string