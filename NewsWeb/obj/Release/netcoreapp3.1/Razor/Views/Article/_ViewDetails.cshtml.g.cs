#pragma checksum "E:\Projects\DevOps\Socialfire\NewsWeb\Views\Article\_ViewDetails.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "97c13c8a42cb7d4ed2808465ce974a804491a8bf"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Article__ViewDetails), @"mvc.1.0.view", @"/Views/Article/_ViewDetails.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\_ViewImports.cshtml"
using NewsWeb;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\_ViewImports.cshtml"
using NewsWeb.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"97c13c8a42cb7d4ed2808465ce974a804491a8bf", @"/Views/Article/_ViewDetails.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"21e7b36322016909d4ca1714ca3f22bc002d55fd", @"/Views/_ViewImports.cshtml")]
    public class Views_Article__ViewDetails : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<NewsWeb.Models.Article>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
            WriteLiteral(@"
<!--Modal Body Start-->

<div class=""modal-content"" style=""width: 900px"">

    <!--Modal Header Start-->
    <div class=""modal-header"">
        <h4 class=""modal-title"">View Detail</h4>
    </div>
    <!--Modal Header End-->


    <div>
        <dl class=""row"">
            <dd class=""col-sm-12"">
                ");
#nullable restore
#line 23 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\Article\_ViewDetails.cshtml"
           Write(Html.DisplayFor(model => model.Title));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <br />\r\n                <i>");
#nullable restore
#line 25 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\Article\_ViewDetails.cshtml"
              Write(Html.DisplayFor(model => model.Date));

#line default
#line hidden
#nullable disable
            WriteLiteral("</i>\r\n            </dd>\r\n            <dd class=\"col-sm-12\">\r\n                <img");
            BeginWriteAttribute("src", " src=\"", 692, "\"", 736, 1);
#nullable restore
#line 28 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\Article\_ViewDetails.cshtml"
WriteAttributeValue("", 698, Html.DisplayFor(model => model.Image), 698, 38, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" height=\"200\" />\r\n            </dd>\r\n            <dd class=\"col-sm-12\">\r\n                <b>");
#nullable restore
#line 31 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\Article\_ViewDetails.cshtml"
              Write(Html.DisplayFor(model => model.Description));

#line default
#line hidden
#nullable disable
            WriteLiteral(" </b>\r\n            </dd>\r\n            <dd class=\"col-sm-12\">\r\n                <!--\r\n     <span style=\"white-space: pre-line\"> ");
#nullable restore
#line 35 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\Article\_ViewDetails.cshtml"
                                     Write(Html.DisplayFor(model => model.Content));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n        -->\r\n            </dd>\r\n            <dd class=\"col-sm-12\">\r\n                ");
#nullable restore
#line 39 "E:\Projects\DevOps\Socialfire\NewsWeb\Views\Article\_ViewDetails.cshtml"
           Write(Html.Raw(@Model.ContentHtml));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </dd>\r\n        </dl>\r\n    </div>\r\n\r\n\r\n\r\n    <!--Modal Body End-->\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<NewsWeb.Models.Article> Html { get; private set; }
    }
}
#pragma warning restore 1591