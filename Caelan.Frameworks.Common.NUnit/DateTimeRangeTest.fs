namespace Caelan.Frameworks.Common.NUnit

open System
open System.Diagnostics
open NUnit.Framework
open Caelan.Frameworks.Common.Classes

[<TestFixture>]
type DateTimeRangeTest() = 
    [<Test>]
    member __.TestRange() = 
        let range = DateTimeRange(DateTime.Today, DateTime.Now)
        Assert.IsTrue(range.IsInRange(DateTime.Now.AddHours(float (-1))))
