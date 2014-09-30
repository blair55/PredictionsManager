namespace PredictionsManager.Web

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Html
open IntelliFactory.WebSharper.JQuery
open PredictionsManager.Domain.Domain
open PredictionsManager.Domain.Presentation

[<JavaScript>]
module JQueryMobile =
    
    let mob = JQuery.Mobile.Mobile.Instance
    let str o = (o.ToString())

    let IndexPage (simplePage: Element) (formTypes: Element) (eventTestPage: Element) = 
        JQuery.Mobile.Mobile.Use()
        let header =
            Div [HTML5.Attr.Data "role" "header"] -< [
                H1 [Text "Index"]
            ]
        let content =
            Div [HTML5.Attr.Data "role" "content"] -< [
                A [HTML5.Attr.Data "role" "button"] -< [
                   HRef "#simplePage" 
                   Text "Simple Page" 
                ]
                |>! OnClick (fun _ _ -> mob.ChangePage(JQuery.Of(simplePage.Body)))
                A [HTML5.Attr.Data "role" "button"] -< [
                   HRef "#formTypes" 
                   Text "Forms"
                ]
                |>! OnClick (fun _ _ -> mob.ChangePage(JQuery.Of(formTypes.Body)))
                A [HTML5.Attr.Data "role" "button"] -< [
                    HRef "#eventTest" 
                    Text "New Events"
                ]
                |>! OnClick (fun _ _ -> mob.ChangePage(JQuery.Of(eventTestPage.Body)))
            ]
        let page = 
            Div [
                Id "indexPage"
                HTML5.Attr.Data "role" "page"
                HTML5.Attr.Data "url" "#indexPage"
                ] -< [
                header
                content
            ]
        page

    //let GetRemoteLtr() = async { return Remoting.getLT() } |> Async.Start

    let SimplePage () = 
        JQuery.Mobile.Mobile.Use()
        let header =
            Div [HTML5.Attr.Data "role" "header"] -< [
                 H1 [Text "Page Title"]
            ]
        let content =
            let head = [ TR[ TD []; TD [Text "Player"]; TD [Text "Points"] ] ]
//            let! ltr = Remoting.getLT()
//            let body = ltr
//                       |> List.map(fun row ->  let playerName = (getPlayerName row.player)
//                                               TR[
//                                                   TD[ Text (str row.position) ]
//                                                   TD[ Text (playerName) ]
//                                                   TD[ Text (str row.points) ]
//                                               ])
//            let combined = List.concat [ head; body ]

            Div [HTML5.Attr.Data "role" "content"] -< [
//                 P [Text "Lorem ipsum dolor sit amet, consectetur adipiscing"]
            ]
        let footer =
            Div [HTML5.Attr.Data "role" "footer"] -< [
                 H4 [Text "Page Footer"]
            ]
        let page = 
            Div [
                Id "simplePage"
                HTML5.Attr.Data "role" "page"
                HTML5.Attr.Data "url" "#simplePage"
                ] -< [
                header
                content
                footer
            ]
        page

    let FormTypes () = 
        JQuery.Mobile.Mobile.Use()
        let home =
            let header =
                Div [HTML5.Attr.Data "role" "header"] -< [
                     H1 [Text "Ice Cream Order Form"]
                ]
            let content =
                let checkbox (name: string) = 
                    Input [
                        Id name
                        Name name
                        Attr.Type "checkbox"
                        Attr.Class "custom"
                        ] -< [
                        Label [Attr.For name] -< [
                            Text name
                        ]
                    ]
                Div [HTML5.Attr.Data "role" "content"] -< [
                    Form [
                        Action "#"
                        Method "get"
                        ] -< [
                        // Name Field
                        Div [HTML5.Attr.Data "role" "fieldcontain"] -< [
                            Label [Attr.For "name"] -< [
                                Text "Your name:"
                            ]
                            Input [
                                Name "name"
                                Attr.Type "text"
                                Attr.Value ""
                            ]
                        ]
                        // Flavour field
                        Div [HTML5.Attr.Data "role" "controlgroup"] -< [
                            Legend [
                                Text "Which flavours would you like?"
                            ]
                            checkbox "Vanilla"
                            checkbox "Chocolate"
                            checkbox "Strawberry"
                        ]
                        // Cones Field
                        Div [HTML5.Attr.Data "role" "fieldcontain"] -< [
                            Label [Attr.For "quantity"] -< [
                                Text "Number of cones:"
                            ]
                            Input [
                                Id   "quantity"
                                Name "quantity"
                                Attr.Type "range"
                                Attr.Value "1"
                                HTML5.Attr.Min "1" 
                                HTML5.Attr.Max "100"
                            ]
                        ]
                        // Sprinkles Field
                        Div [HTML5.Attr.Data "role" "fieldcontain"] -< [
                            Label [Attr.For "sprinkles"] -< [
                                Text "Sprinkles:"
                            ]
                            Select [
                                Id   "sprinkles"
                                Name "sprinkles"
                                HTML5.Attr.Data "role" "slider"
                                ] -< [
                                Default.Tags.Option [
                                    Attr.Value "on"
                                    Text "Yes"
                                ]
                                Default.Tags.Option [
                                    Attr.Value "off"
                                    Text "No"
                                ]
                            ]
                        ]
                        // Store Field
                        Div [HTML5.Attr.Data "role" "fieldcontain"] -< [
                            Label [Attr.For "store"] -< [
                                Text "Collect from store:"
                            ]
                            Select [
                                Id   "store"
                                Name "store"
                                ] -< [
                                Default.Tags.Option [Attr.Value "mainStreet"] -< [
                                    Text "Main Street"
                                ]
                                Default.Tags.Option [Attr.Value "libertyAvenue"] -< [
                                    Text "Liberty Avenue"
                                ]
                                Default.Tags.Option [Attr.Value "circleSquare"] -< [
                                    Text "Circle Square"
                                ]
                                Default.Tags.Option [Attr.Value "angelRoad"] -< [
                                    Text "Angel Road"
                                ]
                            ]
                        ]
                        Div [Attr.Class "ui-body ui-body-b"] -< [
                            FieldSet [Attr.Class "ui-grid-a"] -< [
                                Div [Attr.Class "ui-block-a"] -< [
                                    Button [
                                        Attr.Type "submit"
                                        HTML5.Attr.Data "theme" "a"
                                        ] -< [
                                        Text "Cancel"
                                    ]
                                ]
                                Div [Attr.Class "ui-block-b"] -< [
                                    Button [
                                        Attr.Type "submit"
                                        HTML5.Attr.Data "theme" "a"
                                        ] -< [
                                        Text "Order Ice Cream"
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            let page = 
                Div [
                    Id "formTypes"
                    HTML5.Attr.Data "role" "page"
                    HTML5.Attr.Data "url" "#formTypes"
                    ] -< [
                    header
                    content
                ]
            page
        home

    let EventTestPage () =
        let header =
            Div [HTML5.Attr.Data "role" "header"] -< [
                H1 [Text "Tap me!"]
            ]
        let content =
            Div [HTML5.Attr.Data "role" "content"] -< [
                P [Text "Swipe me!"]
            ]
        let footer =
            Div [HTML5.Attr.Data "role" "footer"] -< [
                H4 [Text "Scroll me!"]
            ]
        
        JQuery.Mobile.Events.Tap.On(JQuery.JQuery.Of(header.Body),
            fun event -> 
                "Tapped on:" + string event.PageX + "," + string event.PageY
                |> JavaScript.Alert
            )
        JQuery.Mobile.Events.Swipe.On(JQuery.JQuery.Of(content.Body),
            fun event -> 
                JavaScript.Alert("Swipped on")
            )
        JQuery.Mobile.Events.ScrollStart.On(JQuery.JQuery.Of(footer.Body),
            fun event -> 
                JavaScript.Alert("Scrolled on")
            )
        let page = 
            Div [
                Id "eventTest"
                HTML5.Attr.Data "role" "page"
                HTML5.Attr.Data "url" "#eventTest"
                ] -< [
                header
                content
                footer
            ]
        page

    let Main () =
        let simplePage = SimplePage ()
        let formTypes = FormTypes ()
        let eventTestPage = EventTestPage ()
        let index = IndexPage simplePage formTypes eventTestPage
        let pages =
            [
                index
                simplePage
                formTypes
                eventTestPage
            ]
        Div pages
        |>! OnAfterRender (fun _ ->
            
            pages |> List.iter (fun elt -> JQuery.Of(elt.Body)
                                        |> JQuery.Mobile.JQuery.Page
                                        |> ignore)
            
            mob.ChangePage(JQuery.Of(simplePage.Body)))
        
type JQueryMobileViewer() =
    inherit Web.Control()

    [<JavaScript>]
    override this.Body = JQueryMobile.Main() :> _
