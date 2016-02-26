namespace Caelan.Frameworks.Common.Classes

open System

type DateTimeRange(start : DateTime, ``end``) = 
    
    /// <summary>
    /// 
    /// </summary>
    member val Start = (start, ``end``) ||> min with get, set
    
    /// <summary>
    /// 
    /// </summary>
    member val End = (start, ``end``) ||> max with get, set
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    member public this.IsInRange date = this.Start <= date && this.End >= date
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dates"></param>
    member public this.IsInRange([<ParamArray>] dates : seq<DateTime>) = dates |> Seq.forall this.IsInRange
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="range"></param>
    member public this.IsInRange(range : DateTimeRange) = this.IsInRange([ range.Start; range.End ])
    
    /// <summary>
    /// 
    /// </summary>
    member public this.ToTimeSpan = this.End - this.Start
