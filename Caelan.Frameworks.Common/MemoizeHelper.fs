namespace Caelan.Frameworks.Common.Helpers

open System.Collections.Generic

[<AutoOpen>]
module MemoizeHelper = 
    let Memoize f = 
        let cache = Dictionary<_, _>()
        fun x -> 
            let mutable ok = Unchecked.defaultof<_>
            if cache.TryGetValue(x, &ok) then ok
            else 
                let res = f x
                cache.[x] <- res
                res
//requires comparable
//    let Memoize f = 
//        let cache = ref Map.empty
//        fun x -> 
//            match (!cache).TryFind(x) with
//            | Some res -> res
//            | None -> 
//                let res = f x
//                cache := (!cache).Add(x, res)
//                res
