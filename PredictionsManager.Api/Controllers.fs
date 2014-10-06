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
    
    [<Route("prediction")>][<HttpPost>]
    member this.AddPrediction (prediction) =
        let handle r = match r with
                        | Success _ -> getOkResponseWithBody ""
                        | Failure s -> unauthorised s

        base.Request |> (getPlayerIdCookie
                        >> bind (trySavePredictionPostModel prediction)
                        >> handle)
        
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
                GameWeekDetailsRowViewModel.fixture=(toFixtureViewModel d.fixture)
                predictionSubmitted=d.prediction.IsSome
                prediction=getVmPred d.prediction
                result={home=fst d.result.score; away=snd d.result.score}
                points=d.points
            }
        let rows = gameWeekDetailRows |> List.map(rowToViewModel)
        { GameWeekDetailsViewModel.gameWeekNo=gameWeekNo; totalPoints=rows|>List.sumBy(fun r -> r.points); rows=rows }
        
    [<Route("openfixtures")>]
    member this.GetOpenFixtures() =
        let returnFixtures p =
            let f = Services.getOpenFixtures p
            getOkResponseWithBody f
        let playerId = getPlayerIdCookie base.Request
        match playerId with
        | Success plId -> returnFixtures plId
        | Failure s -> unauthorised s

        
[<EnableCors("*", "*", "*")>]
[<RoutePrefix("api/admin")>]
type AdminController() =
    inherit ApiController()

    let ignoreArg arg = ()

    [<Route("getnewgameweekno")>]
    member this.GetNewGameWeekNo () =
        base.Request |> (makeSurePlayerIsAdmin
                     >> bind (switch getNewGameWeekNo)
                     >> resultToHttp)

    [<Route("gameweek")>][<HttpPost>]
    member this.CreateGameWeek (gameWeek:GameWeekPostModel) =
        let saveGameWeek() = saveGameWeekPostModel gameWeek
        base.Request |> (makeSurePlayerIsAdmin
                     >> bind saveGameWeek
                     >> resultToHttp)
        
    [<Route("getfixturesawaitingresults")>]
    member this.GetFixturesAwaitingResults() =
        base.Request |> (makeSurePlayerIsAdmin
                     >> bind (switch getFixturesAwaitingResults)
                     >> resultToHttp)

    [<Route("result")>][<HttpPost>]
    member this.AddResult (result:ResultPostModel) =
        let saveResult() = saveResultPostModel result
        base.Request |> (makeSurePlayerIsAdmin
                     >> bind (switch saveResult)
                     >> resultToHttp)


