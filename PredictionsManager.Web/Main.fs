namespace PredictionsManager.Web

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets
open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Presentation

type Action =
    | Home
    | About
    | GameWeeks of int option
    | LeagueTable
    | Player of string
    | PlayerGameWeekScore of string * int

[<AutoOpen>]
module Utils =
    let str o = (o.ToString())

module Controls =

    [<Sealed>]
    type EntryPoint() =
        inherit Web.Control()

        [<JavaScript>]
        override __.Body =
            Client.Main() :> _

module Skin =
    open System.Web

    type Page =
        {
            Title : string
            Body : list<Content.HtmlElement>
        }

    let MainTemplate =
        Content.Template<Page>("~/Main.html")
            .With("title", fun x -> x.Title)
            .With("body", fun x -> x.Body)

    let WithTemplate title body : Content<Action> =
        Content.WithTemplate MainTemplate <| fun context ->
            {
                Title = title
                Body = body context
            }

module Site =

    let ( => ) text url =
        A [HRef url] -< [Text text]

    let Links (ctx: Context<Action>) =
        UL [
            LI ["Home" => ctx.Link Home]
            LI ["About" => ctx.Link About]
            LI ["Gameweeks" => ctx.Link (GameWeeks<|None)]
            LI ["LeagueTable" => ctx.Link LeagueTable]
        ]

    let HomePage =
        Skin.WithTemplate "HomePage" <| fun ctx ->
            [
                Div [Text "HOME"]
                Div [new Controls.EntryPoint()]
                Links ctx
            ]

    let AboutPage =
        Skin.WithTemplate "AboutPage" <| fun ctx ->
            [
                Div [Text "ABOUT"]
                Links ctx
            ]

    let AllGameWeeksPage =
        Skin.WithTemplate "Gws" <| fun ctx ->
            [
                Div [Text "All Gameweeks"]
                getGameWeeks()
                |> List.map(fun gwn ->  let gwInt = getGameWeekNo gwn.number
                                        let gwStr = gwInt.ToString()
                                        LI [ gwStr => ctx.Link (GameWeeks<|Some gwInt) ])
                |> UL
                Div [Text "Add a gw"]
                Links ctx
            ]

    let OneGameWeekPage gwno =
        Skin.WithTemplate "Gw" <| fun ctx ->
            let gameWeekNumberText = "Gameweek #" + str gwno
            [
                Div [Text gameWeekNumberText]
                Links ctx
            ]

    let ThirdWayPage gwnoo =
        match gwnoo with
        | Some gwno -> OneGameWeekPage gwno
        | None -> AllGameWeeksPage

    let LeagueTablePage =
        Skin.WithTemplate "League Table" <| fun ctx ->
            [
                H1 [Text "League Table"]
                getLeagueTableRows()
                |> List.map(fun row -> let playerName = (getPlayerName row.player)
                                       TR[
                                           TD[ Text (str row.position) ]
                                           TD[ playerName => ctx.Link (Player playerName) ]
                                           TD[ Text (str row.points) ]
                                       ])
                |> Table
                Links ctx
            ]

    let PlayerPage player =
        Skin.WithTemplate player <| fun ctx ->
            let gameWeeksAndPoints = getGameWeeksScoreForPlayer player
            [
                H1[Text player]
                gameWeeksAndPoints
                |> List.map(fun row -> let gw = (getGameWeekNo (row|>fst))
                                       LI[
                                           (str gw) => ctx.Link (PlayerGameWeekScore (player, gw))
                                           Span [ Text (str (snd row)) ]
                                       ])
                |> UL
                Links ctx
            ]

    let PlayerGameWeekScorePage player gwno =
        Skin.WithTemplate player <| fun ctx ->
            let playerScoreForGameWeek = getPlayerGameWeekScore player gwno
            let gameWeekDetailsToHtml (row:GameWeekDetailsRow) =
                let stringFixture (fixture:Fixture) = sprintf "%s v %s" fixture.home fixture.away
                let stringScore score = sprintf "%i v %i" (fst score) (snd score)
                let getPredictionDescription (prediction:Prediction option) =
                    match prediction with
                    | Some p -> stringScore p.score
                    | None -> "No prediction submitted"
                TR[
                    TD[Text (stringFixture row.fixture)]
                    TD[Text (stringScore row.result.score)]
                    TD[Text (getPredictionDescription row.prediction)]
                    TD[Text (str row.points)]
                ]

            let head = TR[ TD [Text "Fixture"]; TD [Text "Result"]; TD [Text "Prediction"]; TD [Text "Points"] ]
            let body = playerScoreForGameWeek |> List.map(gameWeekDetailsToHtml)
            let foot = TR[ TD[]; TD[]; TD[]; TD[Text (str (playerScoreForGameWeek|>List.sumBy(fun x->x.points))) ] ]
            let combined = List.concat[ [head]; body; [foot] ]
             
            [
                H1[Text (player + " score for GameWeek " + str gwno)]
                Table combined
                Links ctx
            ]

    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" Home HomePage
            Sitelet.Infer <| function
                                | Home -> HomePage
                                | About -> AboutPage
                                | GameWeeks gwnoo -> ThirdWayPage gwnoo
                                | LeagueTable -> LeagueTablePage
                                | Player player -> PlayerPage player
                                | PlayerGameWeekScore (player,gwno) -> PlayerGameWeekScorePage player gwno
        ]

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [Home; About; ]

type Global() =
    inherit System.Web.HttpApplication()

    member g.Application_Start(sender: obj, args: System.EventArgs) =
        ()

[<assembly: Website(typeof<Website>)>]
do ()