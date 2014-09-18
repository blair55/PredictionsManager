namespace PredictionsManager.Domain

open System

module Domain =

    type FxId = FxId of Guid
    type GwId = GwId of Guid
    type Team = string
    type Deadline = Deadline of DateTime
    type Player = Player of string
    type GwNo = GwNo of int

    // deconstructors
    let getPlayerName (Player name) = name
    let getGameWeekNo (GwNo n) = n

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

    // dtos / viewmodels
    type LeagueTableRow = { position:int; player:Player; points:int }
    type GameWeekAndPoints = GwNo * int
    type GameWeekDetailsRow = { fixture:Fixture; predictionScore:Score; resultScore:Score; points:int }

    // base calculations
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
        let result = results |> List.find(fun r -> r.fixture = prediction.fixture)
        getPointsForPredictionComparedToResult prediction result

    let getTotalPlayerScore (predictions:Prediction list) results player =
            let score = predictions
                        |> List.filter(fun p -> p.player = player)
                        |> List.sumBy(fun p -> getPointsForPrediction p results)
            player, score
    
    // filters
    let getPlayersPredictions (predictions:Prediction list) player = predictions |> List.filter(fun p -> p.player = player)
    let getGameWeekPredictions (predictions:Prediction list) gameWeekNo = predictions |> List.filter(fun p -> p.fixture.gameWeek.number = gameWeekNo)
    let getPlayerGameWeekPredictions (predictions:Prediction list) player gameWeekNo = getGameWeekPredictions (getPlayersPredictions predictions player) gameWeekNo
    let getGameWeekResults (results:Result list) gameWeekNo = results |> List.filter(fun r -> r.fixture.gameWeek.number = gameWeekNo)

    let getAllGameWeeks (results:Result list) =
        results
        |> List.map(fun r -> r.fixture.gameWeek)
        |> Seq.distinctBy(fun gw -> gw)
        |> Seq.toList

    // entry points
    let getAllPlayerScores (predictions:Prediction list) results =
        let getTotalPlayerScorePartialFunc = getTotalPlayerScore predictions results
        let allPlayers = predictions |> List.map(fun p -> p.player) |> Seq.distinctBy(fun p -> p) |> Seq.toList
        allPlayers |> List.map(getTotalPlayerScorePartialFunc)

    let getLeagueTable (predictions:Prediction list) results =
        let playerScores = getAllPlayerScores predictions results
        playerScores
        |> List.sortBy(fun ps -> -(snd ps))
        |> List.mapi(fun i ps -> { position=(i+1); player=(fst ps); points=(snd ps) })

    let getGameWeekPointsForPlayer (predictions:Prediction list) results player gameWeekNo =
        let playerGameWeekPredictions = getPlayerGameWeekPredictions predictions player gameWeekNo
        let points = playerGameWeekPredictions |> List.sumBy(fun p -> getPointsForPrediction p results)
        gameWeekNo, points

    let getAllGameWeekPointsForPlayer (predictions:Prediction list) results player =
        let gameWeeks = getAllGameWeeks results
        let ggwpfp = getGameWeekPointsForPlayer predictions results player
        gameWeeks
        |> List.map(fun gw -> ggwpfp gw.number)
        |> List.sortBy(fun gwp -> getGameWeekNo(fst gwp))
        
    let getGameWeekDetailsForPlayer (predictions:Prediction list) results player gameWeekNo =
        let playerGameWeekPredictions = getPlayerGameWeekPredictions predictions player gameWeekNo
        let gameWeekResults = getGameWeekResults results gameWeekNo
        let getGameWeekDetailsRow prediction result =
            let p = getPointsForPredictionComparedToResult prediction result
            {GameWeekDetailsRow.fixture=result.fixture; predictionScore=prediction.score; resultScore=result.score; points=p }
        gameWeekResults
        |> List.map(fun r -> let prediction = playerGameWeekPredictions |> List.find(fun p -> r.fixture = p.fixture )
                             getGameWeekDetailsRow prediction r)


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
        let generateResult f = {Result.fixture=f; score=getRndScore()}
        fixtures |> List.map(fun f -> generateResult f)

    let fixtures = buildFixtureList teamsList gameWeeksList
    let predictions = buildPredictionsList fixtures playersList
    let results = buildResults fixtures

