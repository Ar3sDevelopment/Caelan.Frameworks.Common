namespace Caelan.Frameworks.Common.Interfaces

type IPasswordHasher = 
    abstract HashPassword : password:string -> string