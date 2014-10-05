namespace PredictionsManager.Domain

open System
open System.IO
open System.Data
open Npgsql

open PredictionsManager.Domain.Domain

module Data =

    let connStr =
        let uriString = "postgres://iobxdsep:sQ5pYLEEjC8Ih6V3OVS_V8F0N67VQ-28@horton.elephantsql.com:5432/iobxdsep"
        let uri = new Uri(uriString)
        let db = uri.AbsolutePath.Trim('/')
        let user = uri.UserInfo.Split(':').[0]
        let passwd = uri.UserInfo.Split(':').[1]
        let port = if uri.Port > 0 then uri.Port else 5432
        String.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}", uri.Host, db, user, passwd, port);
        
    let getConn() = new NpgsqlConnection(connStr)
    let getQuery cn s = new NpgsqlCommand(s, cn)

    let executeNonQuery nq =
        use cn = new NpgsqlConnection(connStr)
        cn.Open()
        let cmd = getQuery cn nq
        let sw = System.Diagnostics.Stopwatch.StartNew()
        cmd.ExecuteNonQuery() |> ignore
        sw.Stop()
        sprintf "%i / %s" sw.ElapsedMilliseconds nq |> log
        //cn.Close()
        
    let rec getListFromReader (r:NpgsqlDataReader) readerToTypeStrategy list =
        match r.Read() with
        | true -> let item = readerToTypeStrategy r
                  let newList = item::list
                  getListFromReader r readerToTypeStrategy newList
        | false -> list

    let executeQuery q readerToTypeStrategy =
        use cn = getConn()
        cn.Open()
        let cmd = getQuery cn q
        let sw = System.Diagnostics.Stopwatch.StartNew()
        let reader = cmd.ExecuteReader()
        sw.Stop()
        sprintf "%s / %i" q sw.ElapsedMilliseconds |> log
        let results = getListFromReader reader readerToTypeStrategy []
        //cn.Close()
        results

    // writing
    
    type PlayerDto = { id:Guid; name:string; role:string; email:string }
    type GameWeekDto = { id:Guid; number:int; description:string; }
    type FixtureDto = { id:Guid; gameWeekId:Guid; home:string; away:string; kickoff:DateTime }
    type ResultDto = { fixtureId:Guid; homeScore:int; awayScore:int }
    type PredictionDto = { fixtureId:Guid; playerId:Guid; homeScore:int; awayScore:int }
    
    let str o = o.ToString()
    let roleToString r = match r with | User -> "User" | Admin -> "Admin"
    let stringToRole s = match s with | "User" -> User | "Admin" -> Admin | _ -> User

    let kostr (k:DateTime) =
        let c = String.Format("{0:u}", k)
        c

    let insertPlayerQuery (p:PlayerDto) = sprintf "insert into players values ('%s', '%s', '%s', '%s')" (str p.id) p.name p.role p.email
    let insertGameWeekQuery (g:GameWeekDto) = sprintf "insert into gameweeks values ('%s', %i, '%s')" (str g.id) g.number g.description
    let insertFixtureQuery (f:FixtureDto) = sprintf "insert into fixtures values ('%s', '%s', '%s', '%s', '%s')" (str f.id) (str f.gameWeekId) f.home f.away (kostr f.kickoff)
    let insertResultQuery (r:ResultDto) = sprintf "insert into results values ('%s', %i, %i)" (str r.fixtureId) (r.homeScore) (r.awayScore)
    let insertPredictionQuery (p:PredictionDto) = sprintf "insert into predictions values ('%s', %i, %i, '%s')" (str p.fixtureId) (p.homeScore) (p.awayScore) (str p.playerId)
    
    let getPlayerDto (p:Player) = { PlayerDto.id=(getPlayerId p.id); name=(p.name); role=(roleToString p.role); email="" }
    let getGameWeekDto (g:GameWeek) = { GameWeekDto.id=(getGwId g.id); number=(getGameWeekNo g.number); description=g.description; }
    let getFixtureDto (f:Fixture) = { FixtureDto.id=(getFxId f.id); gameWeekId=(getGwId f.gameWeek.id); home=f.home; away=f.away; kickoff=f.kickoff }
    let getResultDto (r:Result) = { ResultDto.fixtureId=(getFxId r.fixture.id); homeScore=(fst r.score); awayScore=(snd r.score); }
    let getPredictionDto (p:Prediction) = { PredictionDto.fixtureId=(getFxId p.fixture.id); playerId=(getPlayerId p.player.id); homeScore=(fst p.score); awayScore=(snd p.score); }

    let addPlayer pl = getPlayerDto pl |> insertPlayerQuery |> executeNonQuery
    let addGameWeek g = getGameWeekDto g |> insertGameWeekQuery |> executeNonQuery
    let addFixture f = getFixtureDto f |> insertFixtureQuery |> executeNonQuery
    let addResult r = getResultDto r |> insertResultQuery |> executeNonQuery
    let addPrediction p = getPredictionDto p |> insertPredictionQuery |> executeNonQuery

    // initialising

    let initAll (pl:Player list) (g:GameWeek list) (f:Fixture list) (r:Result list) (p:Prediction list) =
        executeNonQuery "drop table if exists players; create table players (id uuid, name text, role text, email text)"
        executeNonQuery "drop table if exists gameweeks; create table gameweeks (id uuid, number int, description text)"
        executeNonQuery "drop table if exists fixtures; create table fixtures (id uuid, gameWeekid uuid, home text, away text, kickoff timestamp)"
        executeNonQuery "drop table if exists results; create table results (fixtureId uuid, homeScore int, awayScore int)"
        executeNonQuery "drop table if exists predictions; create table predictions (fixtureId uuid, homeScore int, awayScore int, playerId uuid)" 
        pl |> List.iter(addPlayer)
        g |> List.iter(addGameWeek)
        f |> List.iter(addFixture)
        r |> List.iter(addResult)
        p |> List.iter(addPrediction)

    // reading
    
    let readGuidAtPosition (r:NpgsqlDataReader) i = r.GetGuid(i)
    let readStringAtPosition (r:NpgsqlDataReader) i = r.GetString(i)
    let readIntAtPosition (r:NpgsqlDataReader) i = r.GetInt32(i)
    let readDateTimeAtPosition (r:NpgsqlDataReader) i = r.GetDateTime(i)    
    
    let readerToPlayerDto (r:NpgsqlDataReader) =
        { PlayerDto.id = (readGuidAtPosition r 0); name=(readStringAtPosition r 1); role=(readStringAtPosition r 2); email=(readStringAtPosition r 3) }

    let readerToGameWeekDto (r:NpgsqlDataReader) =
        { GameWeekDto.id = (readGuidAtPosition r 0); number=(readIntAtPosition r 1); description=(readStringAtPosition r 2) }
    
    let readerToFixtureDto (r:NpgsqlDataReader) =
        { FixtureDto.id = (readGuidAtPosition r 0); gameWeekId=(readGuidAtPosition r 1); home=(readStringAtPosition r 2); away=(readStringAtPosition r 3); kickoff=(readDateTimeAtPosition r 4) }
    
    let readerToResultDto (r:NpgsqlDataReader) =
        { ResultDto.fixtureId = (readGuidAtPosition r 0); homeScore=(readIntAtPosition r 1); awayScore=(readIntAtPosition r 2) }
        
    let readerToPredictionDto (r:NpgsqlDataReader) =
        { PredictionDto.fixtureId = (readGuidAtPosition r 0); homeScore=(readIntAtPosition r 1); awayScore=(readIntAtPosition r 2); playerId=(readGuidAtPosition r 3) }
    
    
    let readPlayers () =
        (executeQuery "select * from players" readerToPlayerDto)
        |> List.map(fun p -> { Player.id=p.id|>PlId; name=p.name; role=(stringToRole p.role) })

    let readGameWeeks() =
        (executeQuery "select * from gameweeks" readerToGameWeekDto)
        |> List.map(fun gw -> { GameWeek.id=gw.id|>GwId; number=gw.number|>GwNo; description=gw.description })
    
    let readFixtures (gameWeeks:GameWeek list) =
        (executeQuery "select * from fixtures" readerToFixtureDto)
        |> List.map(fun f -> { Fixture.id=f.id|>FxId; gameWeek=(findGameWeekById gameWeeks (f.gameWeekId|>GwId)); home=f.home; away=f.away; kickoff=f.kickoff })
        
    let readResults (fixtures:Fixture list) =
        (executeQuery "select * from results" readerToResultDto)
        |> List.map(fun r -> { Result.fixture=(findFixtureById fixtures (r.fixtureId|>FxId)); score=(r.homeScore, r.awayScore) })
        
    let readPredictions (players:Player list) (fixtures:Fixture list) =
        (executeQuery "select * from predictions" readerToPredictionDto)
        |> List.map(fun r -> { Prediction.fixture=(findFixtureById fixtures (r.fixtureId|>FxId)); score=(r.homeScore, r.awayScore); player=(findPlayerById players (r.playerId|>PlId)) })
    
    let getPlayersAndResultsAndPredictions() =
        let players = readPlayers()
        let gameWeeks = readGameWeeks()
        let fixtures = readFixtures gameWeeks
        let results = readResults fixtures
        let predictions = readPredictions players fixtures
        players, results, predictions

    let getPlayerById playerId =
        readPlayers() |> List.tryFind(fun p -> p.id = playerId)

    let getNewGameWeekNo() =
        let readIntAt0 r = readIntAtPosition r 0
        let gwn = getFirst (executeQuery "select number from gameweeks ORDER BY number DESC LIMIT 1" readIntAt0)
        match gwn with
        | Some n -> n+1
        | None -> 1

    let getGameWeeksAndFixtures() =
        let gameWeeks = readGameWeeks()
        let fixtures = readFixtures gameWeeks
        gameWeeks, fixtures

    let getGameWeeksAndFixturesAndResults() =
        let gameWeeks = readGameWeeks()
        let fixtures = readFixtures gameWeeks
        let results = readResults fixtures
        gameWeeks, fixtures, results

    let getPlayers() = readPlayers()

module DummyData =

    let rnd = new System.Random()
    let teamsList = [ "Arsenal"; "Chelsea"; "Liverpool"; "Everton"; "WestHam"; "Qpr"; "Man Utd"; "Man City"; "Newcastle"; "Sunderland";
                        "Stoke"; "Leicester"; "Spurs"; "Aston Villa"; "West Brom"; "Crystal Palace"; "Hull"; "Burnley"; "Southampton"; "Swansea" ]
    let playersList = [ for p in [ "bob"; "jim"; "tom"; "ian"; "ron"; "jon"; "tim"; "rob"; "len";  ] -> { Player.id=(Guid.NewGuid()|>PlId); name=p; role=User } ]
    let gameWeeksList = [ for i in 1..3 -> { GameWeek.id=Guid.NewGuid()|>GwId; number=(GwNo i); description="" } ]

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

    let ko = new DateTime(2014,10,2);

    let buildFixtureList (teams:string list) gameWeeks =    
        let fixturesPerWeek = teams.Length / 2;
        let buildFixturesForGameWeek teams gw =
            [ for i in 1..fixturesPerWeek -> (  let randomTeams = getTwoDifferentRndTeams teams
                                                { Fixture.id= Guid.NewGuid()|>FxId; gameWeek=gw; home=fst randomTeams; away=snd randomTeams; kickoff=ko }  ) ]
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

    let players = playersList
    let gameWeeks = gameWeeksList
    let fixtures = buildFixtureList teamsList gameWeeksList
    let predictions = buildPredictionsList fixtures playersList
    let results = buildResults fixtures
        