namespace Caelan.Frameworks.Common.Classes
open System

type DateTimeRange(start : DateTime, ``end``) = 
    member val Start = start with get, set
    member val End = ``end`` with get, set
    member public this.IsInRange(date) =
        this.Start >= date && this.End <= date
    member public this.IsInRange([<ParamArray>] dates : seq<DateTime>) =
        dates |> Seq.forall this.IsInRange
    member public this.IsInRange(range : DateTimeRange) =
         this.IsInRange([range.Start; range.End])
    member public this.ToTimeSpan =
        this.End - this.Start
    
