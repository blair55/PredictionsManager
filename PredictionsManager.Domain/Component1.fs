namespace PredictionsManager.Domain

open System

module Domain =

    type FxId = FxId of Guid
    type GwId = GwId of Guid
    type Team = string
    type Deadline = Deadline of DateTime
    type Player = Player of string
    type GwNo = GwNo of int

    let gwid g = GwId g 
    let fxid g = FxId g
    let player p = Player p
    let gwno n = GwNo n
    let dl (s:string) = Convert.ToDateTime(s) |> Deadline

    type Score = int * int
    type GameWeek = {id : GwId; number : GwNo; description : string; deadline : Deadline }
    type Fixture = {id : FxId; gameWeek : GameWeek; home : Team; away : Team }
    type Prediction = { fixture : Fixture; score : Score; player : Player }
    type Result = { fixture : Fixture; score : Score }
    type Outcome = HomeWin | AwayWin | Draw

    let getOutcome score =
        if fst score > snd score then HomeWin
        else if fst score < snd score then AwayWin
        else Draw

    let getPointsForPredictionComparedToResult (prediction:Prediction) result =
        if prediction.score = result.score then 3
        else
            let predictionOutcome = getOutcome prediction.score
            let resultOutcome = getOutcome result.score
            if predictionOutcome = resultOutcome then 1 else 0
        
    let getPointsForPrediction (prediction:Prediction) (results:Result list) =
        // find matching result for prediction
        let result = results |> List.find(fun r -> r.fixture = prediction.fixture)
        getPointsForPredictionComparedToResult prediction result

    let getGameWeekScore (predictions:Prediction list) results player gameWeekNumber =
        let playerPredictionsForGameWeek =
            predictions
            |> List.filter(fun p -> p.player = player)
            |> List.filter(fun p -> p.fixture.gameWeek.number = gameWeekNumber)
        playerPredictionsForGameWeek
            |> List.sumBy(fun p -> getPointsForPrediction p results)
       
    let getTotalPlayerScore (predictions:Prediction list) results player =
            let score = predictions
                        |> List.filter(fun p -> p.player = player)
                        |> List.sumBy(fun p -> getPointsForPrediction p results)
            player, score

    let getAllPlayerScores (predictions:Prediction list) results =
        let getTotalPlayerScorePartialFunc = getTotalPlayerScore predictions results
        let allPlayers = predictions |> List.map(fun p -> p.player) |> Seq.distinctBy(fun p -> p) |> Seq.toList
        allPlayers |> List.map(getTotalPlayerScorePartialFunc)

    let getPlayerName (Player name) = name
    let getGameWeekNo (GwNo n) = n
    
    let printLeagueTableRow position row =
        let playerName = getPlayerName (row|>fst)
        let score = snd row
        printfn "%i. %s - %i" position playerName score

    let displayLeagueTable (predictions:Prediction list) results =
        let playerScores = getAllPlayerScores predictions results
        playerScores
        |> List.sortBy(fun ps -> -(snd ps))
        |> List.iteri(fun i ps -> printLeagueTableRow (i+1) ps)

    let displayLeagueTableForWeek (predictions:Prediction list) results gameWeek =
        let predictionsForGameWeek = predictions |> List.filter(fun p -> p.fixture.gameWeek.number = gameWeek)
        let resultsForGameWeek = results |> List.filter(fun r -> r.fixture.gameWeek.number = gameWeek)
        displayLeagueTable predictionsForGameWeek resultsForGameWeek

    let displayGameWeekDetailsForPlayer (predictions:Prediction list) results player gameWeek =
        let resultsForGameWeek = results |> List.filter(fun r -> r.fixture.gameWeek.number = gameWeek)
        let predictionsForGameWeekForPlayer =
            predictions
            |> List.filter(fun p -> p.fixture.gameWeek.number = gameWeek)
            |> List.filter(fun p -> p.player = player)

        let printGameWeekFixturePoints (r:Result) (p:Prediction option) =
            match p with
            | Some p ->
                let points = getPointsForPredictionComparedToResult p r
                printfn "%sv%s %iv%i %iv%i %i" r.fixture.home r.fixture.away (fst r.score) (snd r.score) (fst p.score) (snd p.score) points
            | None -> printfn "%sv%s %iv%i no prediction" r.fixture.home r.fixture.away (fst r.score) (snd r.score)

        printfn "GameWeek %i" (getGameWeekNo gameWeek)
        resultsForGameWeek
        |> List.iter(fun r -> // find matching prediction by fixture
                              let p = predictionsForGameWeekForPlayer |> List.tryFind(fun pr -> pr.fixture = r.fixture)
                              printGameWeekFixturePoints r p)

        //printfn "total: %i" 
        printfn ""
        ()

    // dummy data

    let rnd = new System.Random()
    let teamsList = [ "Arsenal"; "Chelsea"; "Liverpool"; "Everton"; "WestHam"; "Qpr" ]
    let playersList = [ for p in [ "bob"; "jim"; "tom"; "ian"; "ron"; "jon" ] -> p|>player ]
    let gameWeeksList = [ for i in 1..20 -> { GameWeek.id=Guid.NewGuid()|>gwid; number=(gwno i); description=""; deadline="2014-1-1"|>dl } ]

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
                                                { id= Guid.NewGuid()|>fxid; gameWeek=gw; home=fst randomTeams; away=snd randomTeams }  ) ]
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
        let generateResult f = {fixture=f; score=getRndScore()}
        fixtures |> List.map(fun f -> generateResult f)

    let fixtures = buildFixtureList teamsList gameWeeksList
    let predictions = buildPredictionsList fixtures playersList
    let results = buildResults fixtures

