namespace PredictionsManager.Api

open System
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Routing

type HttpRoute = { controller:string; id:RouteParameter }
type HttpRouteWithPlayer = { controller:string; player:RouteParameter }
type HttpRouteWithPlayerAndGameWeekNo = { controller:string; player:RouteParameter; gameWeekNo:RouteParameter }

type Global() =
    inherit System.Web.HttpApplication() 

    static member RegisterWebApi(config: HttpConfiguration) =
        
        config.MapHttpAttributeRoutes()
        
        config.EnableCors()

        config.Routes.MapHttpRoute(
            "PlayerGameWeekApi",
            "api/Player/{player}/{gameWeekNo}",
            { HttpRouteWithPlayerAndGameWeekNo.controller = "Player"; player=RouteParameter.Optional; gameWeekNo=RouteParameter.Optional } ) |> ignore

        config.Routes.MapHttpRoute(
            "PlayerApi",
            "api/Player/{player}",
            { HttpRouteWithPlayer.controller = "Player"; player=RouteParameter.Optional } ) |> ignore

        config.Routes.MapHttpRoute(
            "DefaultApi", // Route name
            "api/{controller}/{id}", // URL with parameters
            { controller = "{controller}"; id = RouteParameter.Optional } ) |> ignore

        config.Formatters.XmlFormatter.UseXmlSerializer <- true
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()


    member x.Application_Start() =
        GlobalConfiguration.Configure(Action<_> Global.RegisterWebApi)
