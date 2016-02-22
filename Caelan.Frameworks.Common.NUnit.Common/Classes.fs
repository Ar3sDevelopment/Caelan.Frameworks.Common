namespace Caelan.Frameworks.Common.NUnit.Common
open System
open Caelan.Frameworks.ClassBuilder.Attributes

module Classes = 
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

    type TestC() = 
        [<MapEquals>]
        member val A = "" with get, set
        [<MapEquals>]
        [<MapField("B")>]
        member val C = "" with get, set

    type TestD() = 
        [<MapEquals>]
        member val A = "" with get, set
        [<MapEquals>]
        [<MapField("C")>]
        member val B = "" with get, set

    [<MapEquals>]
    type TestE() = 
        [<MapField("C")>]
        member val A = "" with get, set
        [<MapField("D")>]
        member val B = "" with get, set
        [<MapField("E")>]
        member val F = "" with get, set

    [<MapEquals>]
    type TestF() = 
        [<MapField("A")>]
        member val C = "" with get, set
        [<MapField("B")>]
        member val D = "" with get, set
