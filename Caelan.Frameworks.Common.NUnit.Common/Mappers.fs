namespace Caelan.Frameworks.Common.NUnit.Common
open System
open Caelan.Frameworks.Common.Classes
open Classes

module Mappers = 
    type ABMapper() = 
        inherit DefaultMapper<TestA, TestB>()
        override __.Map(source, dest) = 
            base.Map(source, dest) |> ignore
            dest.B <- dest.B + " mapper"
            dest

    type IntFloatMapper() =
        inherit DefaultMapper<int, float>()

        override __.Map(source, destination) =
            (float)source