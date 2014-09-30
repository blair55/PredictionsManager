namespace PredictionsManager.Api

open System

module ViewModels =
    
    let str o = o.ToString()

    type LeagueTableRowViewModel = { position:int; player:string; points:int }
    type LeagueTableViewModel = { rows:LeagueTableRowViewModel list }

    type GameWeekAndPointsViewModel = { gameWeekNo:int; points:int; }
    type PlayerViewModel = { name:string; gameWeekAndPoints:GameWeekAndPointsViewModel list }
    
    type ScoreViewModel = { home:int; away:int; }
    type FixtureViewModel = { home:string; away:string; fxId:string }
    type GameWeekDetailsRowViewModel = { fixture:FixtureViewModel; predictionSubmitted:bool; prediction:ScoreViewModel; result:ScoreViewModel; points:int }
    type GameWeekDetailsViewModel = { playerName:string; gameWeekNo:int; totalPoints:int; rows:GameWeekDetailsRowViewModel list }


module PostModels =

    [<CLIMutable>]
    type CreateGameWeekPostModel = { i:int } 