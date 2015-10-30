namespace Caelan.Frameworks.Common.Classes
open System

type DateTimeRange(start : DateTime, ``end``) = 
    /// <summary>
    /// 
    /// </summary>
    member val Start =
        match start with
        | _ when start > ``end`` -> ``end``
        | _ -> start
        with get, set
    /// <summary>
    /// 
    /// </summary>
    member val End =
        match start with
        | _ when start > ``end`` -> ``start``
        | _ -> ``end``
        with get, set
    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    member public this.IsInRange(date) = this.Start >= date && this.End <= date
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dates"></param>
    member public this.IsInRange([<ParamArray>] dates : seq<DateTime>) = dates |> Seq.forall this.IsInRange
    /// <summary>
    /// 
    /// </summary>
    /// <param name="range"></param>
    member public this.IsInRange(range : DateTimeRange) = this.IsInRange([range.Start; range.End])
    /// <summary>
    /// 
    /// </summary>
    member public this.ToTimeSpan = this.End - this.Start
    
