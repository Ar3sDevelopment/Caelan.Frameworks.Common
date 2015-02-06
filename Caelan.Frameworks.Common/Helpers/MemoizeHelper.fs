namespace Caelan.Frameworks.Common.Helpers

module MemoizeHelper = 
    let Memoize f = 
        let cache = ref Map.empty
        fun x -> 
            match (!cache).TryFind(x) with
            | Some res -> res
            | None -> 
                let res = f x
                cache := (!cache).Add(x, res)
                res
