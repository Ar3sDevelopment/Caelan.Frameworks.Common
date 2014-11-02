#r "System.Data.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"
#r "bin/Debug/Caelan.Frameworks.Common.dll"

open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open Caelan.Frameworks.Common.Interfaces
open Caelan.Frameworks.Common.Classes

[<AllowNullLiteral>]
type TestA() =
    member val A = "test" with get, set

[<AllowNullLiteral>]
type TestB() =
    member val A = "test2" with get, set

type TestMapper() =
    inherit DefaultMapper<TestA, TestB>()

    override __.Map(source, destination) =
        destination.A <- source.A

let testA = TestA()
let testB = Builder<TestA, TestB>.Create((*TestMapper()*)).Build(testA)

testB.A