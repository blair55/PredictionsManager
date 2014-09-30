declare module EditedSitelet {
    module Pages {
        interface Page {
            Title: string;
            Body: __ABBREV.__List.T<any>;
        }
    }
    module MyJQueryContent {
        var Main : {
            (): __ABBREV.__Html.Element;
        };
    }
    interface Action {
    }
    interface MyJQueryMobileEntryPoint {
        get_Body(): __ABBREV.__Html.IPagelet;
    }
    interface Website {
    }
}
declare module __ABBREV {
    
    export import __List = IntelliFactory.WebSharper.List;
    export import __Html = IntelliFactory.WebSharper.Html;
}
