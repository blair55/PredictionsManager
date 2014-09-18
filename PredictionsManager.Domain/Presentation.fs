namespace PredictionsManager.Domain

open PredictionsManager.Domain.Domain

module Presentation =
        
    let printLeagueTable (predictions:Prediction list) results =
        let leagueTable = getLeagueTable predictions results        
        let printLeagueTableRow (row:LeagueTableRow) =
            printfn "%i. %s - %i" row.position (getPlayerName row.player) row.points
        leagueTable |> List.iter(printLeagueTableRow)

    let printGameWeeksScoreForPlayer (predictions:Prediction list) results player =
        let gws = getAllGameWeekPointsForPlayer predictions results player
        let printGameWeekScore gws =
            let gameWeekNo = getGameWeekNo (fst gws)
            printfn "%i - %i" gameWeekNo (snd gws)
        gws |> List.iter printGameWeekScore

    let printGameWeekDetailsForPlayer (predictions:Prediction list) results player gameWeekNo =
        let gameWeekDetailsForPlayer = getGameWeekDetailsForPlayer predictions results player gameWeekNo
        let printGameWeekDetailsRow row =
            let stringFixture fixture = sprintf "%s v %s" fixture.home fixture.away
            let stringScore score = sprintf "%iv%i" (fst score) (snd score)
            let getPredictionDescription (prediction:Prediction option) =
                match prediction with
                | Some p -> stringScore p.score
                | None -> "No prediction submitted"
            printfn "%s %s %s %i" (stringFixture row.fixture) (stringScore row.result.score) (getPredictionDescription row.prediction) row.points
        printfn "GameWeek %i" (getGameWeekNo gameWeekNo)
        gameWeekDetailsForPlayer |> List.iter printGameWeekDetailsRow
        printfn "Total %i" (gameWeekDetailsForPlayer |> List.sumBy(fun gwd -> gwd.points))
        printfn ""

