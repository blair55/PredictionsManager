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
    let getFxId (FxId id) = id.ToString()
    let getGwId (GwId id) = id.ToString()
    let getDeadline (Deadline date) = date

    let dl (s:string) = Convert.ToDateTime(s) |> Deadline

    type Score = int * int
    type GameWeek = { id : GwId; number : GwNo; description : string; deadline : Deadline }
    type Fixture = { id : FxId; gameWeek : GameWeek; home : Team; away : Team }
    type Prediction = { fixture : Fixture; score : Score; player : Player }
    type Result = { fixture : Fixture; score : Score }
    type Outcome = HomeWin | AwayWin | Draw

    // dtos / viewmodels
    type LeagueTableRow = { position:int; predictions:int; player:Player; points:int }
    type GameWeekAndPoints = GwNo * int
    type GameWeekDetailsRow = { fixture:Fixture; prediction:Prediction option; result:Result; points:int }
    type FixtureDetails = { fixture:Fixture; result:Result; predictions:Prediction list }

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
            let predictionsForPlayer = predictions |> List.filter(fun p -> p.player = player)
            let score = predictionsForPlayer |> List.sumBy(fun p -> getPointsForPrediction p results)
            predictionsForPlayer.Length, player, score

    // entry points
    let getAllPlayerScores (predictions:Prediction list) results =
        let getTotalPlayerScorePartialFunc = getTotalPlayerScore predictions results
        let allPlayers = predictions |> List.map(fun p -> p.player) |> Seq.distinctBy(fun p -> p) |> Seq.toList
        allPlayers |> List.map(getTotalPlayerScorePartialFunc)

    let getLeagueTable (predictions:Prediction list) results =
        let playerScores = getAllPlayerScores predictions results
        playerScores
        |> List.sortBy(fun ps -> let (_, _, score) = ps
                                 -score)
        |> List.mapi(fun i ps -> let (predictions, player, score) = ps
                                 { position=(i+1); predictions=predictions; player=player; points=score })

    let getGameWeekPointsForPlayer (predictions:Prediction list) results player gameWeekNo =
        let playerGameWeekPredictions = getPlayerGameWeekPredictions predictions player gameWeekNo
        let points = playerGameWeekPredictions |> List.sumBy(fun p -> getPointsForPrediction p results)
        gameWeekNo, points

    let getAllGameWeekPointsForPlayer (predictions:Prediction list) results player =
        (getAllGameWeeks results)
        |> List.map(fun gw -> getGameWeekPointsForPlayer predictions results player gw.number)
        |> List.sortBy(fun gwp -> getGameWeekNo(fst gwp))
        
    let getGameWeekDetailsForPlayer (predictions:Prediction list) results player gameWeekNo =
        let playerGameWeekPredictions = getPlayerGameWeekPredictions predictions player gameWeekNo
        let gameWeekResults = getGameWeekResults results gameWeekNo
        let getGameWeekDetailsRow (prediction:Prediction option) result =
            let points =
                match prediction with
                | Some p -> getPointsForPredictionComparedToResult p result
                | None -> 0
            {GameWeekDetailsRow.fixture=result.fixture; prediction=prediction; result=result; points=points }
        gameWeekResults
        |> List.map(fun r -> let prediction = playerGameWeekPredictions |> List.tryFind(fun p -> r.fixture = p.fixture )
                             getGameWeekDetailsRow prediction r)
    
    let getPlayerPredictionsForFixture (predictions:Prediction list) (results:Result list) fxid =
        let fixture = (predictions |> List.find(fun p -> p.fixture.id = fxid)).fixture
        let fixtureResult = results |> List.find(fun r -> r.fixture.id = fxid)
        let fixturePredictions = predictions |> List.filter(fun p -> p.fixture.id = fxid)
        { fixture=fixture; result = fixtureResult; predictions=fixturePredictions }