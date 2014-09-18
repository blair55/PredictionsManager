namespace PredictionsManager.Domain

open FSharp.Data
open PredictionsManager.Domain.Domain
open System.IO

module Data =

    //let pCsv = new CsvProvider<"Y:/Code/PredictionsManager/Data/Predictions.csv">()

    [<Literal>]
    let fileName = "Y:/Code/PredictionsManager/Data/Predictions.csv"
    let header = "fixtureId,player,homeScore,awayScore"

    let storePredictions (predictions:Prediction list) =
        let predictionToCsvString (p:Prediction) =
            sprintf "%s,%s,%i,%i" (getFxId p.fixture.id) (getPlayerName p.player) (fst p.score) (snd p.score)
        let l = predictions |> List.map(predictionToCsvString) |> List.toArray
        File.WriteAllLines(fileName, [|header|])
        File.WriteAllLines(fileName, l)

