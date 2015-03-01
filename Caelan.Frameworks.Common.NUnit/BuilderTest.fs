﻿namespace Caelan.Frameworks.Common.NUnit

open Caelan.Frameworks.Common.Attributes
open Caelan.Frameworks.Common.Classes
open NUnit.Framework
open System
open System.Diagnostics

[<MapEquals>]
[<AllowNullLiteral>]
type TestA() = 
    member val A = "" with get, set
    [<MapField("B")>]
    member val C = "" with get, set

[<MapEquals>]
[<AllowNullLiteral>]
type TestB() = 
    member val A = "" with get, set
    [<MapField("C")>]
    member val B = "" with get, set

[<AllowNullLiteral>]
type TestC() = 
    [<MapEquals>]
    member val A = "" with get, set
    [<MapField("B")>]
    member val C = "" with get, set

[<AllowNullLiteral>]
type TestD() = 
    [<MapEquals>]
    member val A = "" with get, set
    [<MapField("C")>]
    member val B = "" with get, set

type ABMapper() = 
    inherit DefaultMapper<TestA, TestB>()
    override __.Map(source, dest) = 
        base.Map(source, dest)
        dest.B <- dest.B + " mapper"

[<TestFixture>]
type BuilderTest() = 
    
    [<Test>]
    member __.TestNoBuilder() = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let a = TestA(A = "test", C = "test")
        let b = TestB(A = "test", B = a.C + " no mapper")
        let str = "A: " + b.A + " B: " + b.B
        Assert.AreEqual(str, "A: test B: test no mapper")
        str |> printfn "%s"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"
    
    [<Test>]
    member __.TestBuilder() = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let a = TestA(A = "test", C = "test")
        let b = Builder.Build(a).To<TestB>()
        let str = "A: " + b.A + " B: " + b.B
        Assert.AreEqual(str, "A: test B: test mapper")
        str |> printfn "%s"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"

    [<Test>]
    member __.TestBuilderEdit() = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let a = TestA(A = "test", C = "test")
        let b = TestB(A = "pippo", B = "pluto")
        Builder.Build(a).To(b)
        let str = "A: " + b.A + " B: " + b.B
        Assert.AreEqual(str, "A: test B: test mapper")
        str |> printfn "%s"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"
    
    [<Test>]
    member __.TestDefaultMapper() = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let b = TestB(A = "test", B = "test2")
        let a = Builder.Build(b).To<TestA>()
        let str = "A: " + a.A + " C: " + a.C
        Assert.AreEqual(str, "A: test C: test2")
        str |> printfn "%s"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"
    
    [<Test>]
    member __.TestList() = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let b = (fun i -> TestB(A = "test", B = "test2")) |> Seq.init 10
        let a = Builder.BuildList(b).ToList<TestA>()
        a |> Seq.iter (fun aItem -> 
                 let str = "A: " + aItem.A + " C: " + aItem.C
                 Assert.AreEqual(str, "A: test C: test2")
                 str |> printfn "%s")
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"

    [<Test>]
    member __.TestDefaultMapperAttributes() = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let b = TestD(A = "test", B = "test2")
        let a = Builder.Build(b).To<TestC>()
        let str = "A: " + a.A + " C: " + a.C
        Assert.AreEqual(str, "A: test C: test2")
        str |> printfn "%s"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"