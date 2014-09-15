namespace PredictionsManager.Domain

open System

type Class1() = 
    member this.X = "F#"

module Domain =

    type Team = string
    type Fixture = Team * Team

    type FixtureBatch = {
        fixtures : Fixture list
        deadline : DateTime }

    type Score = int * int
    type Prediction = Fixture * Score
    type Result = Fixture * Score
    type Outcome = HomeWin | AwayWin | Draw

    let getOutcome score =
        if fst score > snd score then HomeWin
        else if fst score < snd score then AwayWin
        else Draw

    let getPoints prediction result =
        let predictionScore = snd prediction
        let resultScore = snd result
        if predictionScore = resultScore
        then 3
        else
            let predictionOutcome = getOutcome predictionScore
            let resultOutcome = getOutcome resultScore
            if predictionOutcome = resultOutcome
            then 1
            else 0
