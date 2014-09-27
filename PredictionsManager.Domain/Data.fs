namespace PredictionsManager.Domain

open System
open System.IO
open System.Data
open Npgsql

open PredictionsManager.Domain.Domain

module Data =

    let getConn() = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=vagrant; Password=password; Database=vagrant;")

    let getQuery cn s = new NpgsqlCommand(s, cn)

    let executeNonQuery nq =
        let cn = getConn()
        cn.Open()
        let cmd = getQuery cn nq
        cmd.ExecuteNonQuery() |> ignore
        cn.Close()

    // writing
    
    let insertGameWeekQuery (g:GameWeek) = sprintf "insert into gameweeks values ('%s', %i, '%s', '%s')" (getGwId g.id) (getGameWeekNo g.number) g.description ((getDeadline g.deadline).ToString())
    let insertFixtureQuery (f:Fixture) = sprintf "insert into fixtures values ('%s', '%s', '%s', '%s')" (getFxId f.id) (getGwId f.gameWeek.id) f.home f.away
    let insertResultQuery (r:Result) = sprintf "insert into results values ('%s', %i, %i)" (getFxId r.fixture.id) (fst r.score) (snd r.score)
    let insertPredictionQuery (p:Prediction) = sprintf "insert into predictions values ('%s', %i, %i, '%s')" (getFxId p.fixture.id) (fst p.score) (snd p.score) (getPlayerName p.player)
    
    let addGameWeek g = insertGameWeekQuery g |> executeNonQuery
    let addFixture f = insertFixtureQuery f |> executeNonQuery
    let addResult r = insertResultQuery r |> executeNonQuery
    let addPrediction p = insertPredictionQuery p |> executeNonQuery

    // initialising

    let initAll (g:GameWeek list) (f:Fixture list) (r:Result list) (p:Prediction list) =
        executeNonQuery "drop table if exists gameweeks; create table gameweeks (id uuid, number int, description text, deadline timestamp)"
        executeNonQuery "drop table if exists fixtures; create table fixtures (id uuid, gameWeekid uuid, home text, away text)"
        executeNonQuery "drop table if exists results; create table results (fixtureId uuid, homeScore int, awayScore int)"
        executeNonQuery "drop table if exists predictions; create table predictions (fixtureId uuid, homeScore int, awayScore int, player text)" 
        g |> List.iter(addGameWeek)
        f |> List.iter(addFixture)
        r |> List.iter(addResult)
        p |> List.iter(addPrediction)

    // reading
    
    let readGuidAtPosition (r:NpgsqlDataReader) i = r.GetGuid(i)
    let readStringAtPosition (r:NpgsqlDataReader) i = r.GetString(i)
    let readIntAtPosition (r:NpgsqlDataReader) i = r.GetInt32(i)
    let readDateTimeAtPosition (r:NpgsqlDataReader) i = r.GetDateTime(i) 
    
    let rec getListFromReader (r:NpgsqlDataReader) readerToTypeStrategy list =
        match r.Read() with
        | true -> let item = readerToTypeStrategy r
                  let newList = item::list
                  getListFromReader r readerToTypeStrategy newList
        | false -> list
        
    let executeQuery q readerToTypeStrategy =
        let cn = getConn()
        cn.Open()
        let cmd = getQuery cn q
        let reader = cmd.ExecuteReader()
        let results = getListFromReader reader readerToTypeStrategy []
        cn.Close()
        results
    
    type GameWeekDto = { id:Guid; number:int; description:string; deadline:DateTime }
    type FixtureDto = { id:Guid; gameWeekId:Guid; home:string; away:string }
    type ResultDto = { fixtureId:Guid; homeScore:int; awayScore:int }
    type PredictionDto = { fixtureId:Guid; homeScore:int; awayScore:int; player:string }
    
    let readerToGameWeekDto (r:NpgsqlDataReader) =
        { GameWeekDto.id = (readGuidAtPosition r 0); number=(readIntAtPosition r 1); description=(readStringAtPosition r 2); deadline=(readDateTimeAtPosition r 3) }
    
    let readerToFixtureDto (r:NpgsqlDataReader) =
        { FixtureDto.id = (readGuidAtPosition r 0); gameWeekId=(readGuidAtPosition r 1); home=(readStringAtPosition r 2); away=(readStringAtPosition r 3) }
    
    let readerToResultDto (r:NpgsqlDataReader) =
        { ResultDto.fixtureId = (readGuidAtPosition r 0); homeScore=(readIntAtPosition r 1); awayScore=(readIntAtPosition r 2) }
        
    let readerToPredictionDto (r:NpgsqlDataReader) =
        { PredictionDto.fixtureId = (readGuidAtPosition r 0); homeScore=(readIntAtPosition r 1); awayScore=(readIntAtPosition r 2); player=(readStringAtPosition r 3) }
    
    let findGameWeekById (gameWeeks:GameWeek list) id = gameWeeks |> List.find(fun gw -> gw.id = id)
    let findFixtureById (fixtures:Fixture list) id = fixtures |> List.find(fun f -> f.id = id)
    
    let readGameWeeks() =
        (executeQuery "select * from gameweeks" readerToGameWeekDto)
        |> List.map(fun gw -> { GameWeek.id=gw.id|>GwId; number=gw.number|>GwNo; description=gw.description; deadline=gw.deadline|>Deadline })
    
    let readFixtures (gameWeeks:GameWeek list) =
        (executeQuery "select * from fixtures" readerToFixtureDto)
        |> List.map(fun f -> { Fixture.id=f.id|>FxId; gameWeek=(findGameWeekById gameWeeks (f.gameWeekId|>GwId)); home=f.home; away=f.away })
        
    let readResults (fixtures:Fixture list) =
        (executeQuery "select * from results" readerToResultDto)
        |> List.map(fun r -> { Result.fixture=(findFixtureById fixtures (r.fixtureId|>FxId)); score=(r.homeScore, r.awayScore) })
        
    let readPredictions (fixtures:Fixture list) =
        (executeQuery "select * from predictions" readerToPredictionDto)
        |> List.map(fun r -> { Prediction.fixture=(findFixtureById fixtures (r.fixtureId|>FxId)); score=(r.homeScore, r.awayScore); player=r.player|>Player })
    
    let getResultsAndPredictions() =
        let gameWeeks = readGameWeeks()
        let fixtures = readFixtures(gameWeeks)
        let results = readResults(fixtures)
        let predictions = readPredictions(fixtures)
        results, predictions

module DummyData =

    let rnd = new System.Random()
    let teamsList = [ "Arsenal"; "Chelsea"; "Liverpool"; "Everton"; "WestHam"; "Qpr"; "Man Utd"; "Man City"; "Newcastle"; "Sunderland";
                        "Stoke"; "Leicester"; "Spurs"; "Aston Villa"; "West Brom"; "Crystal Palace"; "Hull"; "Burnley"; "Southampton"; "Swansea" ]
    let playersList = [ for p in [ "bob"; "jim"; "tom"; "ian"; "ron"; "jon"; "tim"; "rob"; "len";  ] -> p|>Player ]
    let gameWeeksList = [ for i in 1..38 -> { GameWeek.id=Guid.NewGuid()|>GwId; number=(GwNo i); description=""; deadline="2014-1-1"|>dl } ]

    let getTwoDifferentRndTeams (teams:string list) =
        let getRandomTeamIndex() = rnd.Next(0, teams.Length - 1)
        let homeTeamIndex = getRandomTeamIndex()
        let rec getTeamIndexThatIsnt notThis =
            let index = getRandomTeamIndex()
            match index with
            | i when index=notThis -> getTeamIndexThatIsnt notThis
            | _ -> index
        let awayTeamIndex = getTeamIndexThatIsnt homeTeamIndex
        (teams.[homeTeamIndex], teams.[awayTeamIndex])

    let buildFixtureList (teams:string list) gameWeeks =    
        let fixturesPerWeek = teams.Length / 2;
        let buildFixturesForGameWeek teams gw =
            [ for i in 1..fixturesPerWeek -> (  let randomTeams = getTwoDifferentRndTeams teams
                                                { id= Guid.NewGuid()|>FxId; gameWeek=gw; home=fst randomTeams; away=snd randomTeams }  ) ]
        gameWeeks
            |> List.map(fun gw -> buildFixturesForGameWeek teams gw)    
            |> List.collect(fun f -> f)

    let getRndScore() =
        let getRndGoals() = rnd.Next(0, 4)
        getRndGoals(), getRndGoals()

    let buildPredictionsList (fixtures:Fixture list) players =
        let generatePrediction pl f = { Prediction.fixture=f; score=getRndScore(); player=pl }
        players
        |> List.map(fun pl -> fixtures |> List.map(fun f -> generatePrediction pl f))
        |> List.collect(fun p -> p)
        
    let buildResults (fixtures:Fixture list) =
        let generateResult f = {Result.fixture=f; score=getRndScore()}
        fixtures |> List.map(fun f -> generateResult f)

    let gameWeeks = gameWeeksList
    let fixtures = buildFixtureList teamsList gameWeeksList
    let predictions = buildPredictionsList fixtures playersList
    let results = buildResults fixtures


        
        
        
        
        
        
        
        
        
        
        
        
        
        