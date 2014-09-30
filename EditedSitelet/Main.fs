namespace EditedSitelet

open IntelliFactory.Html
open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.JQuery
open IntelliFactory.WebSharper.Sitelets

type Action = | Home

[<JavaScript>]
module MyJQueryContent =

//    JQuery.Mobile.Mobile.Use()
//    JQuery.Mobile.Mobile.Instance.AutoInitializePage <- false
    
    let Main() =
    
        let page = Div [
                        Id "simplePage"
                        HTML5.Attr.Data "role" "page"
                        HTML5.Attr.Data "url" "#simplePage"
                        ] -< [
                        Div[Text "content"]
                    ]
        Div [page]

        |>! OnBeforeRender (fun _ -> JavaScript.Alert("Before render")
                                     //JavaScript.Alert(JQuery.Mobile.Mobile.Instance.AutoInitializePage.ToString())
                                     JQuery.Mobile.Mobile.Instance.AutoInitializePage <- false)
        

        |>! OnAfterRender (fun _ -> JQuery.Of(page.Body) |> JQuery.Mobile.JQuery.Page |> ignore
                                    //JavaScript.Alert("After render")
                                    JQuery.Mobile.Mobile.Instance.ChangePage(JQuery.Of(page.Body)))

[<Sealed>]
type MyJQueryMobileEntryPoint() =
    inherit Web.Control()

    [<JavaScript>]
    override this.Body = MyJQueryContent.Main() :> _

module Pages =

    type Page =
        {
            Title : string
            Body : list<Content.HtmlElement>
        }

    let MainTemplate =
        IntelliFactory.WebSharper.Sitelets.Content.Template<Page>("~/Main.html")
            .With("title", fun x -> x.Title)
            .With("body", fun x -> x.Body)

    let WithTemplate title body : Content<Action> =
        IntelliFactory.WebSharper.Sitelets.Content.WithTemplate MainTemplate <| fun context ->
            {
                Title = title
                Body = body context
            }

    let HomePage =
        WithTemplate "Home index test" <| fun context ->
            [IntelliFactory.Html.Tags.Div[new MyJQueryMobileEntryPoint()]]

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Sitelet.Content "/" Home Pages.HomePage
        member this.Actions = [Home]

type Global() =
    inherit System.Web.HttpApplication()

    member g.Application_Start(sender: obj, args: System.EventArgs) =
        ()

[<assembly: Website(typeof<Website>)>]
do ()
