namespace Caelan.Frameworks.Common.NUnit

open Caelan.Frameworks.ClassBuilder.Attributes
open Caelan.Frameworks.ClassBuilder.Classes
open Caelan.Frameworks.Common.NUnit.Common.Classes
open NUnit.Framework
open System
open System.Diagnostics

[<TestFixture>]
type BuilderTest() = 
    [<Test>]
    member __.TestNull() =
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let a : TestA = Unchecked.defaultof<TestA>
        let b = Builder.Build(a).To<TestB>()
        Assert.IsNull(b)
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"

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
        Builder.Build(a).To(b) |> ignore
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
        let d = TestD(A = "test", B = "test2")
        let c = Builder.Build(d).To<TestC>()
        let str = "A: " + c.A + " C: " + c.C
        Assert.AreEqual(str, "A: test C: test2")
        str |> printfn "%s"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"

    [<Test>]
    member __.TestDefaultMapperEqualsAttribute() = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let e = TestE(A = "test", B = "test2", F = "test3")
        let f = Builder.Build(e).To<TestF>()
        let str = "C: " + f.C + " D: " + f.D
        Assert.AreEqual(str, "C: test D: test2")
        str |> printfn "%s"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"

    [<Test>]
    member __.TestIntFloat() =
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let source = 10
        let destination = Builder.Build(source).To<float>()
        destination |> printfn "%0.00f"
        stopwatch.Stop()
        stopwatch.ElapsedMilliseconds |> printfn "%dms"