namespace Caelan.Frameworks.Common.Interfaces

type IPasswordHasher = 
    /// <summary>
    /// This function hashes the given password
    /// </summary>
    /// <param name="password">The password to be hashed</param>
    abstract HashPassword : password:string -> string