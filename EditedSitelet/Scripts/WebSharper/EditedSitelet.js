(function()
{
 var Global=this,Runtime=this.IntelliFactory.Runtime,WebSharper,Html,Operators,Default,List,HTML5,alert,jQuery,EditedSitelet,MyJQueryContent;
 Runtime.Define(Global,{
  EditedSitelet:{
   MyJQueryContent:{
    Main:function()
    {
     var page,x,x1;
     page=Operators.add(Default.Div(List.ofArray([Default.Id("simplePage"),HTML5.Attr().NewAttr("data-"+"role","page"),HTML5.Attr().NewAttr("data-"+"url","#simplePage")])),List.ofArray([Default.Div(List.ofArray([Default.Text("content")]))]));
     x=Default.Div(List.ofArray([page]));
     Operators.OnBeforeRender(function()
     {
      alert("Before render");
      jQuery.mobile.autoInitializePage=false;
      return;
     },x);
     x1=x;
     Operators.OnAfterRender(function()
     {
      jQuery(page.Body).page();
      return jQuery.mobile.changePage(jQuery(page.Body));
     },x1);
     return x1;
    }
   },
   MyJQueryMobileEntryPoint:Runtime.Class({
    get_Body:function()
    {
     return MyJQueryContent.Main();
    }
   })
  }
 });
 Runtime.OnInit(function()
 {
  WebSharper=Runtime.Safe(Global.IntelliFactory.WebSharper);
  Html=Runtime.Safe(WebSharper.Html);
  Operators=Runtime.Safe(Html.Operators);
  Default=Runtime.Safe(Html.Default);
  List=Runtime.Safe(WebSharper.List);
  HTML5=Runtime.Safe(Default.HTML5);
  alert=Runtime.Safe(Global.alert);
  jQuery=Runtime.Safe(Global.jQuery);
  EditedSitelet=Runtime.Safe(Global.EditedSitelet);
  return MyJQueryContent=Runtime.Safe(EditedSitelet.MyJQueryContent);
 });
 Runtime.OnLoad(function()
 {
  return;
 });
}());
