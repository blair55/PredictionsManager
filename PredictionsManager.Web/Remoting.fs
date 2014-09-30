namespace PredictionsManager.Web

open IntelliFactory.WebSharper
open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Presentation

module Remoting =

    [<Remote>]
    let Process input =
        async {
            return "You said: " + input
        }

    [<Remote>]
    let getLT() = async { return getLeagueTableRows() }