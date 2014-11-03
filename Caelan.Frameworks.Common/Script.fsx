#r @"bin\Debug\Caelan.Frameworks.Common.dll"

open Caelan.Frameworks.Common.Classes
//open Caelan.Frameworks.Common.Interfaces

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
let testB = Builder<TestA, TestB>.Create(TestMapper()).Build(testA)

testB.A