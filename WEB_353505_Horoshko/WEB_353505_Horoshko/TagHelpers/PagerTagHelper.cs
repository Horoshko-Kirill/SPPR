using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace WEB_353505_Horoshko.TagHelpers
{
    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? Category { get; set; }
        public bool Admin { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.Attributes.SetAttribute("aria-label", "Page navigation");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination justify-content-center");


            int prevPage = CurrentPage > 1 ? CurrentPage - 1 : 1;
            var liPrev = new TagBuilder("li");
            liPrev.AddCssClass("page-item");
            if (CurrentPage == 1) liPrev.AddCssClass("disabled");

            var aPrev = new TagBuilder("a");
            aPrev.AddCssClass("page-link");
            aPrev.Attributes["href"] = GetHref(prevPage);
            aPrev.InnerHtml.AppendHtml("&laquo;");
            liPrev.InnerHtml.AppendHtml(aPrev);
            ul.InnerHtml.AppendHtml(liPrev);


            for (int i = 1; i <= TotalPages; i++)
            {
                var li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (i == CurrentPage) li.AddCssClass("active");

                var a = new TagBuilder("a");
                a.AddCssClass("page-link");
                a.Attributes["href"] = GetHref(i);
                a.InnerHtml.Append(i.ToString());

                li.InnerHtml.AppendHtml(a);
                ul.InnerHtml.AppendHtml(li);
            }


            int nextPage = CurrentPage < TotalPages ? CurrentPage + 1 : TotalPages;
            var liNext = new TagBuilder("li");
            liNext.AddCssClass("page-item");
            if (CurrentPage == TotalPages) liNext.AddCssClass("disabled");

            var aNext = new TagBuilder("a");
            aNext.AddCssClass("page-link");
            aNext.Attributes["href"] = GetHref(nextPage);
            aNext.InnerHtml.AppendHtml("&raquo;");
            liNext.InnerHtml.AppendHtml(aNext);
            ul.InnerHtml.AppendHtml(liNext);

            output.Content.AppendHtml(ul);
        }


        private string GetHref(int page)
        {
            if (Admin)
            {
                return $"/Admin/Index?pageNo={page}&category={Uri.EscapeDataString(Category ?? "")}";
            }
            else
            {
                return $"/Product/Index?pageNo={page}&category={Uri.EscapeDataString(Category ?? "")}";
            }
        }


    }
}
