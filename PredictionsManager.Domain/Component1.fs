namespace PredictionsManager.Domain

open System
open System.Diagnostics

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

    let getPointsForPrediction (prediction:Prediction) (results:Result list) =
        // find matching result for prediction
        let result = results |> List.find(fun r -> r.fixture = prediction.fixture)
        printfn "%A" result
        if prediction.score = result.score then 3
        else
            let predictionOutcome = getOutcome prediction.score
            let resultOutcome = getOutcome result.score
            if predictionOutcome = resultOutcome then 1 else 0
          

    // dummy data

    let someGameWeeks = [
        { id= Guid.NewGuid()|>gwid; number=1|>gwno; description=""; deadline="2014-1-1"|>dl }
        { id= Guid.NewGuid()|>gwid; number=2|>gwno; description=""; deadline="2014-1-3"|>dl }
        { id= Guid.NewGuid()|>gwid; number=3|>gwno; description=""; deadline="2014-1-5"|>dl } ]

    let someFixtures = [
        { id= Guid.NewGuid()|>fxid; gameWeek=someGameWeeks.[0]; home="Arsenal"; away="Liverpool" }
        { id= Guid.NewGuid()|>fxid; gameWeek=someGameWeeks.[0]; home="Arsenal"; away="Liverpool" }
        { id= Guid.NewGuid()|>fxid; gameWeek=someGameWeeks.[1]; home="Arsenal"; away="Liverpool" }
        { id= Guid.NewGuid()|>fxid; gameWeek=someGameWeeks.[1]; home="Arsenal"; away="Liverpool" }
        { id= Guid.NewGuid()|>fxid; gameWeek=someGameWeeks.[2]; home="Arsenal"; away="Liverpool" }
        { id= Guid.NewGuid()|>fxid; gameWeek=someGameWeeks.[2]; home="Arsenal"; away="Liverpool" } ]
    
    let somePredictions = [
         // gw1
         { fixture=someFixtures.[0]; score=(2,1); player="a"|>player }
         { fixture=someFixtures.[1]; score=(4,4); player="a"|>player }
         { fixture=someFixtures.[0]; score=(1,0); player="b"|>player }
         { fixture=someFixtures.[1]; score=(5,1); player="b"|>player }
         { fixture=someFixtures.[0]; score=(1,2); player="c"|>player }
         { fixture=someFixtures.[1]; score=(2,2); player="c"|>player }
         //gw2
         { fixture=someFixtures.[2]; score=(3,5); player="a"|>player }
         { fixture=someFixtures.[3]; score=(5,3); player="a"|>player }
         { fixture=someFixtures.[2]; score=(3,2); player="b"|>player }
         { fixture=someFixtures.[3]; score=(1,4); player="b"|>player }
         { fixture=someFixtures.[2]; score=(0,5); player="c"|>player }
         { fixture=someFixtures.[3]; score=(5,1); player="c"|>player }
         //gw3
         { fixture=someFixtures.[4]; score=(6,7); player="a"|>player }
         { fixture=someFixtures.[5]; score=(3,4); player="a"|>player }
         { fixture=someFixtures.[4]; score=(2,3); player="b"|>player }
         { fixture=someFixtures.[5]; score=(1,1); player="b"|>player }
         { fixture=someFixtures.[4]; score=(5,3); player="c"|>player }
         { fixture=someFixtures.[5]; score=(1,0); player="c"|>player } ]

    let someResults = [
         { fixture=someFixtures.[0]; score=(2,1); }
         { fixture=someFixtures.[1]; score=(2,4); }
         { fixture=someFixtures.[2]; score=(5,2); }
         { fixture=someFixtures.[3]; score=(1,0); }
         { fixture=someFixtures.[4]; score=(0,1); }
         { fixture=someFixtures.[5]; score=(3,2); } ]

    // calculating a game week score for player
        // get player predictions for gw n
        // get all results
        // for each prediction find result & sum points

    let getGameWeekScore (predictions:Prediction list) results player gameWeekNumber =
        let playerPredictionsForGameWeek =
            predictions
            |> List.filter(fun p -> p.player = player)
            |> List.filter(fun p -> p.fixture.gameWeek.number = gameWeekNumber)
        
        let points =
            playerPredictionsForGameWeek
            |> List.sumBy(fun p -> getPointsForPrediction p results)

        points


