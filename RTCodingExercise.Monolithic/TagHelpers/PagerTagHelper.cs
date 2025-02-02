using System.Text.Encodings.Web;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RTCodingExercise.Monolithic.Extensions;

namespace RTCodingExercise.Monolithic.TagHelpers
{
    public class PagerTagHelper : TagHelper
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public int PagesToShow { get; set; } = 5;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.AddClass("pagination", HtmlEncoder.Default);

            var maxBeforeAndAfter = PagesToShow / 2;
            int firstPage = Math.Max(PageNumber - maxBeforeAndAfter, 1);
            int lastPage = Math.Min(firstPage + (PagesToShow - 1), PageCount);

            var urlHelperFactory = new UrlHelperFactory();
            var urlHelper = urlHelperFactory.GetUrlHelper(new ActionContext(ViewContext.HttpContext, ViewContext.RouteData, ViewContext.ActionDescriptor));
            var viewData = ViewContext.RouteData.Values;
            var html = string.Empty;

            var req = ViewContext.HttpContext.Request;
            var qs = HttpUtility.ParseQueryString(req.QueryString.ToString());

            if (PageNumber > 1)
            {
                AddPagingItem("<", $"{PageNumber - 1}", false);
                AddPagingItem("<<", "1", false);
            }

            for(var page = firstPage; page <= lastPage; ++page)
            {
                var strPage = page.ToString();

                AddPagingItem(strPage, strPage, page == PageNumber);
            }

            if (PageNumber < PageCount)
            {
                AddPagingItem(">", $"{PageNumber + 1}", false);
                AddPagingItem(">>", PageCount.ToString(), false);
            }

            // Get the child content and render it inside the <ul>
            output.Content.SetHtmlContent(html);

            void AddPagingItem(string label, string page, bool isSelected) {
                var li = new TagBuilder("li");
                var selected = isSelected ? " active" : "";

                var a = new TagBuilder("a");
                a.AddCssClass("page-link");

                qs.Set("page", page);
                a.Attributes.Add("href", urlHelper.Action(viewData["action"]?.ToString()) + $"?{qs}");

                a.InnerHtml.AppendHtml(label);

                li.AddCssClass($"page-item{selected}");

                li.InnerHtml.AppendHtml(a.ToHtmlString());

                html += li.ToHtmlString();
            }
        }
    }
}
