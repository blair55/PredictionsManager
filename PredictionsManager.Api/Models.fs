namespace PredictionsManager.Api

open System
open PredictionsManager.Domain.Common
open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Data
open Newtonsoft.Json

[<AutoOpen>]
module ViewModels =
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type PlayerViewModel = { id:string; name:string; isAdmin:bool }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type LeagueTableRowViewModel = { position:int; player:PlayerViewModel; points:int }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type LeagueTableViewModel = { rows:LeagueTableRowViewModel list }

    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type PlayerGameWeekViewModel = { gameWeekNo:int; points:int; }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type PlayerGameWeeksViewModel = { gameWeekAndPoints:PlayerGameWeekViewModel list }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type ScoreViewModel = { home:int; away:int; }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type FixtureViewModel = { home:string; away:string; fxId:string; kickoff:DateTime; gameWeekNumber:int }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type GameWeekDetailsRowViewModel = { fixture:FixtureViewModel; predictionSubmitted:bool; prediction:ScoreViewModel; result:ScoreViewModel; points:int }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type GameWeekDetailsViewModel = { gameWeekNo:int; totalPoints:int; rows:GameWeekDetailsRowViewModel list }

    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type OpenGameWeeksViewModel = { rows: int list }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type OpenFixturesViewModel = { rows: FixtureViewModel list }

    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type FixturesAwaitingResultsViewModel = { rows: FixtureViewModel list }

    let getPlayerViewModel (p:Player) = { id=getPlayerId p.id|>str; name=p.name; isAdmin=(p.role=Admin) } 

    let toFixtureViewModel (f:Fixture) = {home=f.home; away=f.away; fxId=(getFxId f.id)|>str; kickoff=f.kickoff; gameWeekNumber=(getGameWeekNo f.gameWeek.number) }

[<AutoOpen>]
module PostModels =

    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type FixturePostModel = { home:string; away:string; kickOff:string }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type GameWeekPostModel = { number:int; fixtures:FixturePostModel list }

    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type ResultPostModel = { fixtureId:string; score:ScoreViewModel }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type PredictionPostModel = { fixtureId:string; score:ScoreViewModel }

module Services =
    
    let getNewGameWeekNo() = getNewGameWeekNo()
    let longStrToDateTime (s:string) =
        let d = s.Split('+').[0];
        Convert.ToDateTime(d)

    // build gameweek from post model
    let tryCreateGameWeekFromPostModel (gwpm:GameWeekPostModel) (gwid:GwId) =
        let areAllFixturesInFuture = gwpm.fixtures |> List.exists(fun f -> f.kickOff|>longStrToDateTime < DateTime.Now) = false
        match areAllFixturesInFuture with
        | true -> Success { GameWeek.id=gwid; number=(GwNo gwpm.number); description="" }
        | false -> Failure "Fixture dates must be in the future"

    // is gameweek number correct
    let checkGameWeekNo (gw:GameWeek) =
        let expected = getNewGameWeekNo()
        if (getGameWeekNo gw.number) = expected then Success gw else Failure (sprintf "get week number should be %i" expected)

    // build fixtures
    let createFixtures (gwpm:GameWeekPostModel) (gw:GameWeek) =
        let toKickOff s = (s|>longStrToDateTime)
        let toFixture f = { Fixture.id=Guid.NewGuid()|>FxId; gameWeek=gw; home=f.home; away=f.away; kickoff=f.kickOff|>toKickOff }
        gwpm.fixtures |> List.map(toFixture)

    // try save gameweek
    let tryToSaveGameWeek gw =
        let addGw() = addGameWeek gw; gw
        tryToWithReturn addGw

    let tryToSaveFixtures fixtures =
        let addFixtures() =
            fixtures |> List.iter(fun f -> addFixture f)
            ()
        tryToWithReturn addFixtures

    // try save fixtures

    let saveGameWeekPostModel (gwpm:PostModels.GameWeekPostModel) =
        let newGameWeekId = Guid.NewGuid()|>GwId;        
        newGameWeekId |> ((tryCreateGameWeekFromPostModel gwpm)
                        >> bind checkGameWeekNo
                        >> bind tryToSaveGameWeek
                        >> bind (switch (createFixtures gwpm))
                        >> bind tryToSaveFixtures)

    let getOpenFixtures (playerId:string) =
        let plId = PlId (sToGuid playerId)
        let (_, fixtures) = getGameWeeksAndFixtures()
        let (players, _, predictions) = getPlayersAndResultsAndPredictions()
        let rows = (getOpenFixturesForPlayer predictions fixtures players plId) |> List.map(toFixtureViewModel)
        { OpenFixturesViewModel.rows=rows }
        
    let getFixturesAwaitingResults() =
        let (_, fixtures, results) = getGameWeeksAndFixturesAndResults()
        let rows = getFixturesAwaitingResults fixtures results |> List.map(toFixtureViewModel)
        { FixturesAwaitingResultsViewModel.rows = rows }

    let saveResultPostModel (rpm:ResultPostModel) =
        let fxId = FxId (sToGuid rpm.fixtureId)
        let (_, fixtures) = getGameWeeksAndFixtures()
        let fixture = findFixtureById fixtures fxId
        let result = { Result.fixture=fixture; score=(rpm.score.home,rpm.score.away)  }
        addResult result


    let trySavePredictionPostModel (ppm:PredictionPostModel) (playerId:string) =
        let tryCreatePrediction() =
            let plId = PlId (sToGuid playerId)
            let fxId = FxId (sToGuid ppm.fixtureId)
            let (_, fixtures) = getGameWeeksAndFixtures()
            let players = getPlayers()
            let fixture = findFixtureById fixtures fxId
            let player = findPlayerById players plId
            let isPredictionForFixtureInTheFuture = fixture.kickoff < DateTime.Now
            match isPredictionForFixtureInTheFuture with
            | true -> Success { Prediction.fixture=fixture; player=player; score=(ppm.score.home,ppm.score.away) }
            | false -> Failure "Fixture has already kicked off" 
        
        let tryAddPrediction p =
            let addPredictionWithReturn() =
                addPrediction p; ()
            tryToWithReturn addPredictionWithReturn
        
        () |> (tryCreatePrediction >> bind tryAddPrediction)
        