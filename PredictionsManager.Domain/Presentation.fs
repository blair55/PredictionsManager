namespace PredictionsManager.Domain

open System
open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Data

module Presentation =
        
    let printLeagueTable (predictions:Prediction list) results =
        let leagueTable = getLeagueTable predictions results        
        let printLeagueTableRow (row:LeagueTableRow) =
            printfn "%i. %s - %i" row.position (row.player.name) row.points
        leagueTable |> List.iter(printLeagueTableRow)

//    let printGameWeeksScoreForPlayer players (predictions:Prediction list) results playerId =
//        let gws = getAllGameWeekPointsForPlayer players predictions results playerId
//        let printGameWeekScore gws =
//            let gameWeekNo = getGameWeekNo (fst gws)
//            printfn "%i - %i" gameWeekNo (snd gws)
//        gws |> List.iter printGameWeekScore

    let printGameWeekDetailsForPlayer (predictions:Prediction list) results playerId gameWeekNo =
        let gameWeekDetailsForPlayer = getGameWeekDetailsForPlayer predictions results playerId (gameWeekNo|>GwNo)
        let printGameWeekDetailsRow (row:GameWeekDetailsRow) =
            let stringFixture (fixture:Fixture) = sprintf "%s v %s" fixture.home fixture.away
            let stringScore score = sprintf "%iv%i" (fst score) (snd score)
            let getPredictionDescription (prediction:Prediction option) =
                match prediction with
                | Some p -> stringScore p.score
                | None -> "No prediction submitted"
            printfn "%s %s %s %i" (stringFixture row.fixture) (stringScore row.result.score) (getPredictionDescription row.prediction) row.points
        printfn "GameWeek %i" gameWeekNo
        gameWeekDetailsForPlayer |> List.iter printGameWeekDetailsRow
        printfn "Total %i" (gameWeekDetailsForPlayer |> List.sumBy(fun gwd -> gwd.points))
        printfn ""


    let getPlayer playerId =
        Data.getPlayerById (playerId|>PlId)

    let getGameWeeks() =
        Data.readGameWeeks() |> List.sortBy(fun gw -> gw.number)

    let getLeagueTableRows() =
        let (_, results, predictions) = Data.getPlayersAndResultsAndPredictions()
        getLeagueTable predictions results

    let getGameWeeksPointsForPlayer playerId =
        let (players, results, predictions) = Data.getPlayersAndResultsAndPredictions()
        let player = findPlayerById players (playerId|>PlId)
        getAllGameWeekPointsForPlayer predictions results player

    let getPlayerGameWeekPoints playerId gameWeekNo =
        let (players, results, predictions) = Data.getPlayersAndResultsAndPredictions()
        let player = findPlayerById players (playerId|>PlId)
        getGameWeekDetailsForPlayer predictions results player (gameWeekNo|>GwNo)

    let getFixtureDetail fxid =
        let (_, results, predictions) = Data.getPlayersAndResultsAndPredictions()
        getPlayerPredictionsForFixture predictions results (FxId fxid)


    