namespace Caelan.Frameworks.Common.Helpers

open System.Collections.Generic

[<AutoOpen>]
module MemoizeHelper = 
    let Memoize f = 
        let cache = Dictionary<_, _>()
        fun x -> 
            match x |> cache.TryGetValue with
            | (true, value) -> value
            | (false, _) -> 
                cache.[x] <- x |> f
                cache.[x]