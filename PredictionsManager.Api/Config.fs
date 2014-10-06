namespace PredictionsManager.Api

open System
open System.Net.Http
open System.Net.Http.Headers
open System.Web
open System.Web.Http
open System.Web.Routing

type HttpRoute = { controller:string; id:RouteParameter }

type Config() =

    static member RegisterWebApi(config: HttpConfiguration) =
        config.MapHttpAttributeRoutes()
        config.EnableCors()
        //config.Formatters.XmlFormatter.UseXmlSerializer <- false
        config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html") );
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver
            <- Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
