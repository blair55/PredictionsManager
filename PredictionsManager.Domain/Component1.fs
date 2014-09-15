namespace PredictionsManager.Domain

open System

module Domain =

    type FxId = FxId of Guid
    type GwId = GxId of Guid
    type Team = string

    type GameWeek = {
        id : GwId
        number : int
        description : string
        deadline : DateTime }

    type Fixture = {
        id : FxId
        gameWeek : GameWeek
        home : Team
        away : Team }

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
        if predictionScore = resultScore then 3
        else
            let predictionOutcome = getOutcome predictionScore
            let resultOutcome = getOutcome resultScore
            if predictionOutcome = resultOutcome then 1 else 0
    
    
    // calculating a game week score for player
        // get player predictions for gw n
        // get all results
        // for each prediction find result & sum points