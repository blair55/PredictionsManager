namespace PredictionsManager.Web

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets

open PredictionsManager.Domain.Presentation

type Action =
    | Home
    | About
    | GameWeeks
    

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
            LI ["Gameweeks" => ctx.Link GameWeeks]
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


    let GameWeeksPage =
        Skin.WithTemplate "Gws" <| fun ctx ->
            [
                Div [Text "All Gameweeks"]
                getGameWeeks()
                |> List.mapi(fun i _ -> LI [Text (i.ToString())])
                |> UL
                
                Div [Text "Add a gw"]
                
            ]

    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" Home HomePage
            Sitelet.Content "/About" About AboutPage
            Sitelet.Content "/GameWeeks" GameWeeks GameWeeksPage
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
