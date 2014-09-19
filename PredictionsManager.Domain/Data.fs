namespace PredictionsManager.Domain

open FSharp.Data
open PredictionsManager.Domain.Domain
open System
open System.IO

module Data =

    [<Literal>]
    let dir = "Y:/Code/PredictionsManager/Data/"
    [<Literal>]
    let fFilePath = dir + "fixtures.csv"
    [<Literal>]
    let rFilePath = dir + "results.csv"
    [<Literal>]
    let pFilePath = dir + "predictions.csv"
    
    let fHeader = "id,gameWeekNo,home,away"
    let rHeader = "fixtureId,homeScore,awayScore"
    let pHeader = "fixtureId,player,homeScore,awayScore"
    let fToCsvString (f:Fixture) = sprintf "%s,%i,%s,%s" (getFxId f.id) (getGameWeekNo f.gameWeek.number) f.home f.away
    let rToCsvString (r:Result) = sprintf "%s,%i,%i" (getFxId r.fixture.id) (fst r.score) (snd r.score)
    let pToCsvString (p:Prediction) = sprintf "%s,%s,%i,%i" (getFxId p.fixture.id) (getPlayerName p.player) (fst p.score) (snd p.score)
        
    let initList l filePath header map =
        let lines = l |> List.map(map)
        let linesAndHead = header::lines |> List.toArray
        File.WriteAllLines(filePath, linesAndHead)

    let initFixtures (f:Fixture list) = initList f fFilePath fHeader fToCsvString
    let initResults (r:Result list) = initList r rFilePath rHeader rToCsvString
    let initPredictions (p:Prediction list) = initList p pFilePath pHeader pToCsvString

    let initAll (f:Fixture list) (r:Result list) (p:Prediction list) =
        initFixtures f
        initResults r
        initPredictions p

    // type Stocks = CsvProvider<"../data/MSFT.csv">
    // let msft = Stocks.Load("http://ichart.finance.yahoo.com/table.csv?s=MSFT")

    type fCsv = CsvProvider<fFilePath>
    type rCsv = CsvProvider<rFilePath>
    type pCsv = CsvProvider<pFilePath>

    let fData = fCsv.Load(fFilePath)
    let rData = rCsv.Load(rFilePath)
    let pData = pCsv.Load(pFilePath)
    
    let getGuid s = Guid.Parse s

    let getFixtures() =
        fData.Rows |> Seq.map(fun x -> { Fixture.id=(getGuid x.id); gameWeek })
        