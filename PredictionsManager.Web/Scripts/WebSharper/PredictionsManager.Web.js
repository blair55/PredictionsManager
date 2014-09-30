(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,WebSharper,Html,Default,List,PredictionsManager,Web,Client,EventsPervasives,Concurrency,Remoting,Operators,HTML5,jQuery,alert,JQueryMobile,Seq,T,String;
 Runtime.Define(Global,{
  PredictionsManager:{
   Web:{
    Client:{
     Main:function()
     {
      var input,label,x,arg00;
      input=Default.Input(List.ofArray([Default.Text("")]));
      label=Default.Div(List.ofArray([Default.Text("")]));
      x=Default.Button(List.ofArray([Default.Text("Click")]));
      arg00=function()
      {
       return function()
       {
        return Client.Start(input.get_Value(),function(out)
        {
         return label.set_Text(out);
        });
       };
      };
      EventsPervasives.Events().OnClick(arg00,x);
      return Default.Div(List.ofArray([input,label,x]));
     },
     Start:function(input,k)
     {
      return Concurrency.Start(Concurrency.Delay(function()
      {
       return Concurrency.Bind(Remoting.Async("PredictionsManager.Web:0",[input]),function(arg101)
       {
        return Concurrency.Return(k(arg101));
       });
      }));
     }
    },
    Controls:{
     EntryPoint:Runtime.Class({
      get_Body:function()
      {
       return Client.Main();
      }
     })
    },
    JQueryMobile:{
     EventTestPage:function()
     {
      var header,content,footer;
      header=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","header")])),List.ofArray([Default.H1(List.ofArray([Default.Text("Tap me!")]))]));
      content=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","content")])),List.ofArray([Default.P(List.ofArray([Default.Text("Swipe me!")]))]));
      footer=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","footer")])),List.ofArray([Default.H4(List.ofArray([Default.Text("Scroll me!")]))]));
      jQuery(header.Body).on("tap",function(event)
      {
       return alert("Tapped on:"+Global.String(event.pageX)+","+Global.String(event.pageY));
      });
      jQuery(content.Body).on("swipe",function()
      {
       return alert("Swipped on");
      });
      jQuery(footer.Body).on("scrollstart",function()
      {
       return alert("Scrolled on");
      });
      return Operators.add(Default.Div(List.ofArray([Default.Id("eventTest"),HTML5.Attr().NewAttr("data-"+"role","page"),HTML5.Attr().NewAttr("data-"+"url","#eventTest")])),List.ofArray([header,content,footer]));
     },
     FormTypes:function()
     {
      var header,checkbox,content,arg101,arg102,arg103,arg104,arg105,arg106;
      header=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","header")])),List.ofArray([Default.H1(List.ofArray([Default.Text("Ice Cream Order Form")]))]));
      checkbox=function(name)
      {
       var arg10;
       arg10=List.ofArray([Default.Attr().NewAttr("for",name)]);
       return Operators.add(Default.Input(List.ofArray([Default.Id(name),Default.Name(name),Default.Attr().NewAttr("type","checkbox"),Default.Attr().Class("custom")])),List.ofArray([Operators.add(Default.Tags().NewTag("label",arg10),List.ofArray([Default.Text(name)]))]));
      };
      arg101=List.ofArray([Default.Attr().NewAttr("for","name")]);
      arg102=List.ofArray([Default.Text("Which flavours would you like?")]);
      arg103=List.ofArray([Default.Attr().NewAttr("for","quantity")]);
      arg104=List.ofArray([Default.Attr().NewAttr("for","sprinkles")]);
      arg105=List.ofArray([Default.Attr().NewAttr("for","store")]);
      arg106=List.ofArray([Default.Attr().Class("ui-grid-a")]);
      content=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","content")])),List.ofArray([Operators.add(Default.Form(List.ofArray([Default.Action("#"),Default.Attr().NewAttr("method","get")])),List.ofArray([Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","fieldcontain")])),List.ofArray([Operators.add(Default.Tags().NewTag("label",arg101),List.ofArray([Default.Text("Your name:")])),Default.Input(List.ofArray([Default.Name("name"),Default.Attr().NewAttr("type","text"),Default.Attr().NewAttr("value","")]))])),Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","controlgroup")])),List.ofArray([Default.Tags().NewTag("legend",arg102),checkbox("Vanilla"),checkbox("Chocolate"),checkbox("Strawberry")])),Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","fieldcontain")])),List.ofArray([Operators.add(Default.Tags().NewTag("label",arg103),List.ofArray([Default.Text("Number of cones:")])),Default.Input(List.ofArray([Default.Id("quantity"),Default.Name("quantity"),Default.Attr().NewAttr("type","range"),Default.Attr().NewAttr("value","1"),HTML5.Attr().NewAttr("min","1"),HTML5.Attr().NewAttr("max","100")]))])),Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","fieldcontain")])),List.ofArray([Operators.add(Default.Tags().NewTag("label",arg104),List.ofArray([Default.Text("Sprinkles:")])),Operators.add(Default.Select(List.ofArray([Default.Id("sprinkles"),Default.Name("sprinkles"),HTML5.Attr().NewAttr("data-"+"role","slider")])),List.ofArray([Default.Tags().NewTag("option",List.ofArray([Default.Attr().NewAttr("value","on"),Default.Text("Yes")])),Default.Tags().NewTag("option",List.ofArray([Default.Attr().NewAttr("value","off"),Default.Text("No")]))]))])),Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","fieldcontain")])),List.ofArray([Operators.add(Default.Tags().NewTag("label",arg105),List.ofArray([Default.Text("Collect from store:")])),Operators.add(Default.Select(List.ofArray([Default.Id("store"),Default.Name("store")])),List.ofArray([Operators.add(Default.Tags().NewTag("option",List.ofArray([Default.Attr().NewAttr("value","mainStreet")])),List.ofArray([Default.Text("Main Street")])),Operators.add(Default.Tags().NewTag("option",List.ofArray([Default.Attr().NewAttr("value","libertyAvenue")])),List.ofArray([Default.Text("Liberty Avenue")])),Operators.add(Default.Tags().NewTag("option",List.ofArray([Default.Attr().NewAttr("value","circleSquare")])),List.ofArray([Default.Text("Circle Square")])),Operators.add(Default.Tags().NewTag("option",List.ofArray([Default.Attr().NewAttr("value","angelRoad")])),List.ofArray([Default.Text("Angel Road")]))]))])),Operators.add(Default.Div(List.ofArray([Default.Attr().Class("ui-body ui-body-b")])),List.ofArray([Operators.add(Default.Tags().NewTag("fieldset",arg106),List.ofArray([Operators.add(Default.Div(List.ofArray([Default.Attr().Class("ui-block-a")])),List.ofArray([Operators.add(Default.Button(List.ofArray([Default.Attr().NewAttr("type","submit"),HTML5.Attr().NewAttr("data-"+"theme","a")])),List.ofArray([Default.Text("Cancel")]))])),Operators.add(Default.Div(List.ofArray([Default.Attr().Class("ui-block-b")])),List.ofArray([Operators.add(Default.Button(List.ofArray([Default.Attr().NewAttr("type","submit"),HTML5.Attr().NewAttr("data-"+"theme","a")])),List.ofArray([Default.Text("Order Ice Cream")]))]))]))]))]))]));
      return Operators.add(Default.Div(List.ofArray([Default.Id("formTypes"),HTML5.Attr().NewAttr("data-"+"role","page"),HTML5.Attr().NewAttr("data-"+"url","#formTypes")])),List.ofArray([header,content]));
     },
     IndexPage:function(simplePage,formTypes,eventTestPage)
     {
      var header,content,x,arg00,x1,arg001,x2,arg002;
      header=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","header")])),List.ofArray([Default.H1(List.ofArray([Default.Text("Index")]))]));
      x=Operators.add(Default.A(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","button")])),List.ofArray([Default.HRef("#simplePage"),Default.Text("Simple Page")]));
      arg00=function()
      {
       return function()
       {
        return JQueryMobile.mob().changePage(jQuery(simplePage.Body));
       };
      };
      EventsPervasives.Events().OnClick(arg00,x);
      x1=Operators.add(Default.A(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","button")])),List.ofArray([Default.HRef("#formTypes"),Default.Text("Forms")]));
      arg001=function()
      {
       return function()
       {
        return JQueryMobile.mob().changePage(jQuery(formTypes.Body));
       };
      };
      EventsPervasives.Events().OnClick(arg001,x1);
      x2=Operators.add(Default.A(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","button")])),List.ofArray([Default.HRef("#eventTest"),Default.Text("New Events")]));
      arg002=function()
      {
       return function()
       {
        return JQueryMobile.mob().changePage(jQuery(eventTestPage.Body));
       };
      };
      EventsPervasives.Events().OnClick(arg002,x2);
      content=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","content")])),List.ofArray([x,x1,x2]));
      return Operators.add(Default.Div(List.ofArray([Default.Id("indexPage"),HTML5.Attr().NewAttr("data-"+"role","page"),HTML5.Attr().NewAttr("data-"+"url","#indexPage")])),List.ofArray([header,content]));
     },
     Main:function()
     {
      var simplePage,formTypes,eventTestPage,pages,x;
      simplePage=JQueryMobile.SimplePage();
      formTypes=JQueryMobile.FormTypes();
      eventTestPage=JQueryMobile.EventTestPage();
      pages=List.ofArray([JQueryMobile.IndexPage(simplePage,formTypes,eventTestPage),simplePage,formTypes,eventTestPage]);
      x=Default.Div(pages);
      Operators.OnAfterRender(function()
      {
       Seq.iter(function(elt)
       {
        jQuery(elt.Body).page();
       },pages);
       return JQueryMobile.mob().changePage(jQuery(simplePage.Body));
      },x);
      return x;
     },
     SimplePage:function()
     {
      var header,content,footer;
      header=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","header")])),List.ofArray([Default.H1(List.ofArray([Default.Text("Page Title")]))]));
      List.ofArray([Default.TR(List.ofArray([Default.TD(Runtime.New(T,{
       $:0
      })),Default.TD(List.ofArray([Default.Text("Player")])),Default.TD(List.ofArray([Default.Text("Points")]))]))]);
      content=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","content")])),Runtime.New(T,{
       $:0
      }));
      footer=Operators.add(Default.Div(List.ofArray([HTML5.Attr().NewAttr("data-"+"role","footer")])),List.ofArray([Default.H4(List.ofArray([Default.Text("Page Footer")]))]));
      return Operators.add(Default.Div(List.ofArray([Default.Id("simplePage"),HTML5.Attr().NewAttr("data-"+"role","page"),HTML5.Attr().NewAttr("data-"+"url","#simplePage")])),List.ofArray([header,content,footer]));
     },
     mob:Runtime.Field(function()
     {
      return jQuery.mobile;
     }),
     str:function(o)
     {
      return String(o);
     }
    },
    JQueryMobileViewer:Runtime.Class({
     get_Body:function()
     {
      return JQueryMobile.Main();
     }
    })
   }
  }
 });
 Runtime.OnInit(function()
 {
  WebSharper=Runtime.Safe(Global.IntelliFactory.WebSharper);
  Html=Runtime.Safe(WebSharper.Html);
  Default=Runtime.Safe(Html.Default);
  List=Runtime.Safe(WebSharper.List);
  PredictionsManager=Runtime.Safe(Global.PredictionsManager);
  Web=Runtime.Safe(PredictionsManager.Web);
  Client=Runtime.Safe(Web.Client);
  EventsPervasives=Runtime.Safe(Html.EventsPervasives);
  Concurrency=Runtime.Safe(WebSharper.Concurrency);
  Remoting=Runtime.Safe(WebSharper.Remoting);
  Operators=Runtime.Safe(Html.Operators);
  HTML5=Runtime.Safe(Default.HTML5);
  jQuery=Runtime.Safe(Global.jQuery);
  alert=Runtime.Safe(Global.alert);
  JQueryMobile=Runtime.Safe(Web.JQueryMobile);
  Seq=Runtime.Safe(WebSharper.Seq);
  T=Runtime.Safe(List.T);
  return String=Runtime.Safe(Global.String);
 });
 Runtime.OnLoad(function()
 {
  JQueryMobile.mob();
  return;
 });
}());
