namespace Caelan.Frameworks.ClassBuilder
open System

module Extenders =
    type Activator with
        static member CreateInstance<'T>(``type``: Type, [<ParamArray>] arguments : obj[]) =
            Activator.CreateInstance(``type``, arguments) :?> 'T