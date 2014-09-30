namespace PredictionsManager.Api

open System
open System.Net
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Routing

open PredictionsManager.Api.ViewModels
open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Presentation
        
type LeagueTableController() =
    inherit ApiController()
    
    member this.Get() =
        let rows = getLeagueTableRows() |> List.map(fun r -> { LeagueTableRowViewModel.position=r.position; player=str<|(getPlayerName r.player); points=r.points })
        { LeagueTableViewModel.rows=rows }

type PlayerController() =
    inherit ApiController()

    member this.Get(player:string) =
        let gameWeeksPoints = (getGameWeeksPointsForPlayer player) |> List.map(fun (gw,p) -> {GameWeekAndPointsViewModel.gameWeekNo=getGameWeekNo gw; points=p })
        { PlayerViewModel.name=player; gameWeekAndPoints=gameWeeksPoints }

    member this.GetGw (player:string) (gameWeekNo:int) =
        let gameWeekDetailRows = (getPlayerGameWeekPoints player gameWeekNo)
        let rowToViewModel (d:GameWeekDetailsRow) =
            let getVmPred (pred:Prediction option) =
                match pred with
                | Some p -> { ScoreViewModel.home=fst p.score;away=snd p.score }
                | None -> { ScoreViewModel.home=0;away=0 }
            {
                GameWeekDetailsRowViewModel.fixture={home=d.fixture.home; away=d.fixture.away; fxId=getFxId d.fixture.id}
                predictionSubmitted=d.prediction.IsSome
                prediction=getVmPred d.prediction
                result={home=fst d.result.score; away=snd d.result.score}
                points=d.points
            }
        let rows = gameWeekDetailRows |> List.map(rowToViewModel)
        { GameWeekDetailsViewModel.playerName=player; gameWeekNo=gameWeekNo; totalPoints=rows|>List.sumBy(fun r -> r.points); rows=rows }

    