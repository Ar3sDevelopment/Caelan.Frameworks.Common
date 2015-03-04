namespace Caelan.Frameworks.Common.NUnit.Common
open System
open Caelan.Frameworks.Common.Classes
open Classes

module Mappers = 
    type ABMapper() = 
        inherit DefaultMapper<TestA, TestB>()
        override __.Map(source, dest) = 
            base.Map(source, dest)
            dest.B <- dest.B + " mapper"