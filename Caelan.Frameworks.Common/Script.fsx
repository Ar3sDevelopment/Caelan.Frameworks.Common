// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

//open Caelan.Frameworks.Common

// Define your library scripting code here

#r @"..\packages\AutoMapper.3.2.1\lib\net40\AutoMapper.dll"
#r @"..\packages\AutoMapper.3.2.1\lib\net40\AutoMapper.Net4.dll"

open AutoMapper
open AutoMapper.Internal

Unchecked.defaultof<int>

//Mapper.CreateMap<System.Int32, System.String>()

//let item = Array.find (fun item -> item.SourceType == typedefof<'TSource> && item.Destination == typedefof<'TDestination>) Mapper.GetAllTypeMaps()
//item.GetUnmappedPropertyNames() |> Array.map (fun prop -> expression.ForMember(prop, fun opt -> opt.Ignore()))