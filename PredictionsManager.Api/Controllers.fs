namespace PredictionsManager.Api

open System
open System.Linq
open System.Net
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Routing
open System.Web.Http.Cors
open System.Net.Http.Headers
open PredictionsManager.Domain.Common
open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Presentation
open PredictionsManager.Api.ViewModels
open PredictionsManager.Api.PostModels
open PredictionsManager.Api.Services

[<EnableCors("*", "*", "*")>]
[<RoutePrefix("api")>]
type HomeController() =
    inherit ApiController()

    let playerIdCookieName = "playerId"

    let getCookieValue (request:HttpRequestMessage) key =
        let cookie = request.Headers.GetCookies(key) |> Seq.toList |> getFirst
        match cookie with
        | Some c -> Success c.[key].Value
        | None -> Failure "No cookie found"

    let logPlayerIn (request:HttpRequestMessage) (player:PlayerViewModel) =
        let nd d = new Nullable<DateTimeOffset>(d)
        let c = new CookieHeaderValue(playerIdCookieName, player.id)
        let july1025 = new DateTime(2015, 7, 1)        
        let uri s = new Uri(s)
        let r = new HttpResponseMessage(HttpStatusCode.Redirect)
        c.Expires <- new DateTimeOffset(july1025) |> nd
        c.Path <- "/"
        //c.Domain <- "localhost:9000"
        
        r.Headers.AddCookies([c])
        r.Headers.Location <- request.RequestUri.GetLeftPart(UriPartial.Authority)|>uri
        r

    // get player cookie value
    let getPlayerIdCookie r =
        getCookieValue r playerIdCookieName

    // convert value to guid
    let convertStringToGuid v =
        let (isParsed, guid) = trySToGuid v
        if isParsed then Success guid else Failure (sprintf "could not convert %s to guid" v)

    // get player from guid
    let getPlayerFromGuid guid =
        let player = getPlayer guid
        match player with
        | Some p -> Success (p|>getPlayerViewModel)
        | None -> Failure (sprintf "no player found matching id %s" (str guid))

    let getOkResponseWithBody (body:'T) =
        let response = new HttpResponseMessage(HttpStatusCode.OK)
        response.Content <- new ObjectContent<'T>(body, new Formatting.JsonMediaTypeFormatter())
        response
    
    let unauthorised msg =
        let response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        response.Content <- new StringContent(msg)
        response

    let getWhoAmIResponse result =
        match result with
        | Success player -> getOkResponseWithBody player
        | Failure msg -> unauthorised msg

    let doLogin req result =
        match result with
        | Success player -> logPlayerIn req player
        | Failure msg -> unauthorised msg

    [<Route("whoami")>]
    member this.GetWhoAmI() =
        base.Request |> (getPlayerIdCookie
        >> bind convertStringToGuid
        >> bind getPlayerFromGuid
        >> getWhoAmIResponse)

    [<Route("auth/{playerId:Guid}")>]
    member this.GetAuthenticate (playerId:Guid) =
        let req = base.Request
        playerId |> (getPlayerFromGuid >> doLogin req)
    
    [<Route("leaguetable")>]
    member this.GetLeagueTable () =
        let rows = getLeagueTableRows() |> List.map(fun r -> { LeagueTableRowViewModel.position=r.position; player=getPlayerViewModel r.player; points=r.points })
        { LeagueTableViewModel.rows=rows }

    [<Route("player/{playerId:Guid}")>]
    member this.GetPlayer (playerId:Guid) =
        let gameWeeksPoints = (getGameWeeksPointsForPlayer playerId) |> List.map(fun (gw,p) -> { PlayerGameWeekViewModel.gameWeekNo=getGameWeekNo gw; points=p })
        { PlayerGameWeeksViewModel.gameWeekAndPoints=gameWeeksPoints }

    [<Route("playergameweek/{playerId:Guid}/{gameWeekNo:int}")>]
    member this.GetPlayerGameWeek (playerId:Guid) (gameWeekNo:int) =
        let gameWeekDetailRows = (getPlayerGameWeekPoints playerId gameWeekNo)
        let rowToViewModel (d:GameWeekDetailsRow) =
            let getVmPred (pred:Prediction option) =
                match pred with
                | Some p -> { ScoreViewModel.home=fst p.score;away=snd p.score }
                | None -> { ScoreViewModel.home=0;away=0 }
            {
                GameWeekDetailsRowViewModel.fixture={home=d.fixture.home; away=d.fixture.away; fxId=(getFxId (d.fixture.id)).ToString(); kickoff=d.fixture.kickoff }
                predictionSubmitted=d.prediction.IsSome
                prediction=getVmPred d.prediction
                result={home=fst d.result.score; away=snd d.result.score}
                points=d.points
            }
        let rows = gameWeekDetailRows |> List.map(rowToViewModel)
        { GameWeekDetailsViewModel.gameWeekNo=gameWeekNo; totalPoints=rows|>List.sumBy(fun r -> r.points); rows=rows }
        

[<EnableCors("*", "*", "*")>]
[<RoutePrefix("api/admin")>]
type AdminController() =
    inherit ApiController()

    //getnewgameweekno
    [<Route("getnewgameweekno")>]
    member this.GetNewGameWeekNo () =
        getNewGameWeekNo()

    [<Route("gameweek")>][<HttpPost>]
    member this.CreateGameWeek (gameWeek:GameWeekPostModel) =
        let r = saveGameWeekPostModel gameWeek
        match r with
        | Success _ -> new HttpResponseMessage(HttpStatusCode.OK)
        | Failure s -> new HttpResponseMessage(HttpStatusCode.InternalServerError)
        

