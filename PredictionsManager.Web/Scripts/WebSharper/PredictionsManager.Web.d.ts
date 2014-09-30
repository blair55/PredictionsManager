declare module PredictionsManager {
    module Web {
        module Skin {
            interface Page {
                Title: string;
                Body: __ABBREV.__List.T<any>;
            }
        }
        module Controls {
            interface EntryPoint {
                get_Body(): __ABBREV.__Html.IPagelet;
            }
        }
        module JQueryMobile {
            var str : {
                <_M1>(o: _M1): string;
            };
            var IndexPage : {
                (simplePage: __ABBREV.__Html.Element, formTypes: __ABBREV.__Html.Element, eventTestPage: __ABBREV.__Html.Element): __ABBREV.__Html.Element;
            };
            var SimplePage : {
                (): __ABBREV.__Html.Element;
            };
            var FormTypes : {
                (): __ABBREV.__Html.Element;
            };
            var EventTestPage : {
                (): __ABBREV.__Html.Element;
            };
            var Main : {
                (): __ABBREV.__Html.Element;
            };
            var mob : {
                (): __ABBREV.__Mobile.Mobile;
            };
        }
        module Client {
            var Start : {
                (input: string, k: {
                    (x: string): void;
                }): void;
            };
            var Main : {
                (): __ABBREV.__Html.Element;
            };
        }
        interface JQueryMobileViewer {
            get_Body(): __ABBREV.__Html.IPagelet;
        }
        interface Action {
        }
        interface Website {
        }
    }
}
declare module __ABBREV {
    
    export import __List = IntelliFactory.WebSharper.List;
    export import __Html = IntelliFactory.WebSharper.Html;
    export import __Mobile = IntelliFactory.WebSharper.JQuery.Mobile;
}
