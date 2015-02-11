namespace Caelan.Frameworks.Common.Classes
open System
open System.Reflection
open Autofac

type BuilderModule internal (assembly : Assembly) =
    inherit Module()
    override __.Load builder =
        builder.RegisterAssemblyTypes(assembly).Where(fun t -> t.Name.EndsWith("Mapper")).AsImplementedInterfaces() |> ignore
        builder.RegisterGeneric(typedefof<Builder<_,_>>).AsSelf() |> ignore
    public new() =
        BuilderModule(Assembly.GetCallingAssembly())

type BuilderContainer() as this =
    inherit ContainerBuilder()
    let assembly = Assembly.GetCallingAssembly()

    do this.RegisterModule(BuilderModule(assembly)) |> ignore
