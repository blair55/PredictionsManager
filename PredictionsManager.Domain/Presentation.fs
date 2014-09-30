namespace PredictionsManager.Domain

open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Data

module Presentation =
        
    let printLeagueTable (predictions:Prediction list) results =
        let leagueTable = getLeagueTable predictions results        
        let printLeagueTableRow (row:LeagueTableRow) =
            printfn "%i. %s - %i" row.position (getPlayerName row.player) row.points
        leagueTable |> List.iter(printLeagueTableRow)

    let printGameWeeksScoreForPlayer (predictions:Prediction list) results player =
        let gws = getAllGameWeekPointsForPlayer predictions results (player|>Player)
        let printGameWeekScore gws =
            let gameWeekNo = getGameWeekNo (fst gws)
            printfn "%i - %i" gameWeekNo (snd gws)
        gws |> List.iter printGameWeekScore

    let printGameWeekDetailsForPlayer (predictions:Prediction list) results player gameWeekNo =
        let gameWeekDetailsForPlayer = getGameWeekDetailsForPlayer predictions results (player|>Player) (gameWeekNo|>GwNo)
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

    let getGameWeeks() =
        Data.readGameWeeks() |> List.sortBy(fun gw -> gw.number)

    let getLeagueTableRows() =
        let (results, predictions) = Data.getResultsAndPredictions()
        getLeagueTable predictions results

    let getGameWeeksPointsForPlayer player =
        let (results, predictions) = Data.getResultsAndPredictions()
        getAllGameWeekPointsForPlayer predictions results (player|>Player)

    let getPlayerGameWeekPoints player gameWeekNo =
        let (results, predictions) = Data.getResultsAndPredictions()
        getGameWeekDetailsForPlayer predictions results (player|>Player) (gameWeekNo|>GwNo)

    let getFixtureDetail fxid =
        let (results, predictions) = Data.getResultsAndPredictions()
        getPlayerPredictionsForFixture predictions results (FxId fxid)
