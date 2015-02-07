namespace Caelan.Frameworks.Common
open System

module Extenders =
    type Activator with
        static member CreateInstance<'T>(``type``: Type, [<ParamArray>] arguments : obj[]) =
            Activator.CreateInstance(``type``, arguments) :?> 'T