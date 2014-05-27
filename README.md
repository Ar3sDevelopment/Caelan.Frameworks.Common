#Caelan.Frameworks.Common NuGet Package

##What is it?
My Framework.Common package is an utility framework for those projects who wants to separate DAL from BIZ or want to abstract code and doesn't want to create many objects for creating a DTO from an Entity or similar cases. It provides a BaseBuilder abstract classes that creates an object of type TDestination from a TSource. It uses AutoMapper, but I often need some more than a simple map so I've created a class that works fine with it. There's a PasswordHelper for creating hashed password in SHA512, simply provide a Salt and a DefaultPassword and you're done.

##Why use it?
If you need to create object from other objects you can use the BaseBuilder and inherit from him to create your own builders with your customization or remove AutoMapper support.
