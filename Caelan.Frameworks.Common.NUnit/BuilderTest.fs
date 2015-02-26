namespace Caelan.Frameworks.Common.NUnit
open System
open System.Diagnostics
open NUnit.Framework
open Caelan.Frameworks.Common.Classes
open Caelan.Frameworks.Common.Attributes

[<MapEquals>]
type TestA() =
    member val A = "" with get, set

    [<MapField("B")>]
    member val C = "" with get, set

[<MapEquals>]
type TestB() =
    member val A = "" with get, set

    [<MapField("C")>]
    member val B = "" with get, set

[<TestFixture>]
type BuilderTest() = 
    [<Test>]
    member __.TestNoBuilder() =
        let stopwatch = Stopwatch()
        stopwatch.Start()
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"