namespace Caelan.Frameworks.Common.Classes

open System

type DateTimeRange(start : DateTime, ``end``) = 
    
    /// <summary>
    /// Start date of range
    /// </summary>
    member val Start = (start, ``end``) ||> min with get, set
    
    /// <summary>
    /// End date of range
    /// </summary>
    member val End = (start, ``end``) ||> max with get, set
    
    /// <summary>
    /// Check if requested date is in range
    /// </summary>
    /// <param name="date">The date that needs check</param>
    member public this.IsInRange date = this.Start <= date && this.End >= date
    
    /// <summary>
    /// Check if requested dates are in range
    /// </summary>
    /// <param name="dates">The dates list that needs check</param>
    member public this.IsInRange([<ParamArray>] dates : seq<DateTime>) = dates |> Seq.forall this.IsInRange
    
    /// <summary>
    /// Check if requested range is in range
    /// </summary>
    /// <param name="range">The range that needs check</param>
    member public this.IsInRange(range : DateTimeRange) = this.IsInRange([ range.Start; range.End ])
    
    /// <summary>
    /// Conversion to TimeSpan from Range
    /// </summary>
    member public this.ToTimeSpan = this.End - this.Start
