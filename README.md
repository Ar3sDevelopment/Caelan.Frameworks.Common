[![Stories in Ready](https://badge.waffle.io/Ar3sDevelopment/Caelan.Frameworks.Common.png?label=ready&title=Ready)](https://waffle.io/Ar3sDevelopment/Caelan.Frameworks.Common)
#Common Framework
The Caelan.Frameworks.Common NuGet Package
If you need support or you want to contact me I'm [CaelanIt](https://twitter.com/CaelanIt) on Twitter
##Build##
AppVeyor (Windows): [![Build status](https://ci.appveyor.com/api/projects/status/0wi8iemmxy4xu986?svg=true)](https://ci.appveyor.com/project/matteobruni/caelan-frameworks-common)

Travis CI (Unix Mono): [![Build Status](https://travis-ci.org/Ar3sDevelopment/Caelan.Frameworks.Common.svg?branch=master)](https://travis-ci.org/Ar3sDevelopment/Caelan.Frameworks.Common)
##Requests##
Pull Requests: [![Issue Stats](http://issuestats.com/github/Ar3sDevelopment/Caelan.Frameworks.Common/badge/pr)](http://issuestats.com/github/Ar3sDevelopment/Caelan.Frameworks.Common)

Issues: [![Issue Stats](http://issuestats.com/github/Ar3sDevelopment/Caelan.Frameworks.Common/badge/issue)](http://issuestats.com/github/Ar3sDevelopment/Caelan.Frameworks.Common)

##What is it?##
My Framework.Common package is an utility framework for delegating the object mapping to a specific class or simply for a password hasher class.

##`Builder` and `IMapper<TSource, TDestination>`##
You can use `Builder` class for map a source to a destination or create a new object from the source like this:
```csharp
var userDto = Builder.Build(user).To<UserDTO>(); //user is a User instance
```
But if a `IMapper<User, UserDTO>` implementation is missing it will build an empty `UserDTO`.
`DefaultMapper<TSource, TDestination>` is an abstract class that prepare a simple `IMapper<TSource, TDestination>` implementation, you have only to define a body for `Map(TSource source, TDestination destination)` method, like this:
```csharp
public class UserDTOMapper : DefaultMapper<User, UserDTO>
{
  public override void Map(User source, UserDTO destination)
  {
    base.Map(source, destination)
    //body here like
    //destination.Member = source.Member;
    //nothing to return
  }
}
```
And you're done. `Builder` class searches for it in assemblies and use it without you have to do nothing more than the `Builder` syntax showed before.
If `Builder` class can't find it or you have multiple mapper for same types you can specify the mapper like this:
```csharp
var mapper = new UserDTOMapper();
//if the mapper has some custom property you can initialize them here
var userDto = Builder.Build(user).To<UserDTO>(mapper); //user is a User instance
```

##`IPasswordEncryptor`##
`IPasswordEncryptor` is a simple interface with two methods:
```csharp
string EncryptPassword(string password)
string DecryptPassword(string password)
```
And you can inherit from this for a custom password encryptor and reference it by the interface.
I created a small `PasswordEncryptor` class that provides *Base64* crypting.
`PasswordEncryptor` is very simple, you can instantiate like this:
```csharp
const string default = "Def4ult";
var encryptor = new PasswordEncryptor(default);
//and now you know how to encrypt
encryptor.EncryptPassword("123456789");
```

##`IPasswordHasher`##
`IPasswordHasher` is a simple interface with two methods:
```csharp
string HashPassword(string password)
```
And you can inherit from this for a custom password encryptor and reference it by the interface.
I created a small `PasswordHasher` class that provides *SHA512* hasing with a salt.
`PasswordEncryptor` is very simple, you can instantiate like this:
```csharp
const string salt = "Salty";
const string default = "Def4ult";
var encryptor = new PasswordHasher(salt, default);
//and now you know how to encrypt
encryptor.HashPassword("123456789");
```
