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
    type FixtureViewModel = { home:string; away:string; fxId:string; kickoff:DateTimeOffset }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type GameWeekDetailsRowViewModel = { fixture:FixtureViewModel; predictionSubmitted:bool; prediction:ScoreViewModel; result:ScoreViewModel; points:int }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type GameWeekDetailsViewModel = { gameWeekNo:int; totalPoints:int; rows:GameWeekDetailsRowViewModel list }

    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type OpenGameWeeksViewModel = { rows: int list }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type OpenFixturesViewModel = { rows: FixtureViewModel list }

    let getPlayerViewModel (p:Player) = { id=getPlayerId p.id|>str; name=p.name; isAdmin=(p.role=Admin) } 

    let toFixtureViewModel (f:Fixture) = {home=f.home; away=f.away; fxId=(getFxId (f.id)).ToString(); kickoff=f.kickoff }

[<AutoOpen>]
module PostModels =

    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type FixturePostModel = { home:string; away:string; kickOff:DateTime }
    
    [<CLIMutable>][<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
    type GameWeekPostModel = { number:int; fixtures:FixturePostModel list }



module Services =
    
    let getNewGameWeekNo() = getNewGameWeekNo()
        
    // build gameweek from post model
    let createGameWeekFromPostModel (gwpm:GameWeekPostModel) (gwid:GwId) =
        { GameWeek.id=gwid; number=(GwNo gwpm.number); description="" }

    // is gameweek number correct
    let checkGameWeekNo (gw:GameWeek) =
        let expected = getNewGameWeekNo()
        if (getGameWeekNo gw.number) = expected then Success gw else Failure (sprintf "get week number should be %i" expected)

    // build fixtures
    let createFixtures (gwpm:GameWeekPostModel) (gw:GameWeek) =
        gwpm.fixtures |> List.map(fun f -> { Fixture.id=Guid.NewGuid()|>FxId; gameWeek=gw; home=f.home; away=f.away; kickoff=(new DateTimeOffset(f.kickOff)) } )

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
        newGameWeekId |> (switch (createGameWeekFromPostModel gwpm)
                        >> bind checkGameWeekNo
                        >> bind tryToSaveGameWeek
                        >> bind (switch (createFixtures gwpm))
                        >> bind tryToSaveFixtures)

    let getOpenGameWeeks() =
        let (gameWeeks, fixtures) = getGameWeeksAndFixtures()
        let rows = (getOpenGameWeeks gameWeeks fixtures) |> List.map(fun gw -> getGameWeekNo gw.number)
        { OpenGameWeeksViewModel.rows=rows }

    let getOpenFixtures gwno =
        let gameWeekNo = GwNo gwno
        let (_, fixtures) = getGameWeeksAndFixtures()
        let rows = getOpenFixturesForGameWeek fixtures gameWeekNo |> List.map(toFixtureViewModel)
        { OpenFixturesViewModel.rows=rows }
        