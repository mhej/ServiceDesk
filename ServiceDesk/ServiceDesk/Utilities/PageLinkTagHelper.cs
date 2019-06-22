using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ServiceDesk.Models;

namespace ServiceDesk.Utilities
{
    /// <summary>Provides Tag Helper for pagination. To be used in Index Views.</summary>
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        /// <summary>Initializes a new instance of the <see cref="PageLinkTagHelper"/> class.</summary>
        /// <param name="helperFactory">The helper factory instance used for dependency injection.</param>
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        /// <summary>Gets or sets the view context.</summary>
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        /// <summary>Gets or sets the <see cref="PagingInfo"/>.</summary>
        public PagingInfo PageModel { get; set; }
        /// <summary>Gets or sets the specified action of the controller.</summary>
        public string PageAction { get; set; }
        /// <summary>Gets or sets a value indicating whether <see cref="PageClass"/> is enabled.</summary>
        public bool PageClassesEnabled { get; set; }
        /// <summary>Gets or sets Bootstrap CSS class.</summary>
        public string PageClass { get; set; }
        /// <summary>Gets or sets Bootstrap CSS class for pagination button.</summary>
        public string PageClassNormal { get; set; }
        /// <summary>Gets or sets Bootstrap CSS class for pagination button of selected page.</summary>
        public string PageClassSelected { get; set; }

        /// <summary>Synchronously executes the <see cref="T:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper"/> with the given <paramref name="context" /> and
        /// <paramref name="output" />.</summary>
        /// <param name="context">Contains information associated with the current HTML tag.</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.urlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }

    }
}

