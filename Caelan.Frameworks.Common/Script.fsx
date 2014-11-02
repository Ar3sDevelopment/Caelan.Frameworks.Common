#r "System.Data.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"

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

let testA = TestA()
let testB = Builder<TestA, TestB>.Create().Build(testA)