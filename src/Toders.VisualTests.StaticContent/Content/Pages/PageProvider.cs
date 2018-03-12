using System.Collections.Generic;
using AlloyTemplates.Models.Pages;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Cache;
using Toders.VisualTests.StaticContent.Content.Assets;
using Toders.VisualTests.StaticContent.Models;

namespace Toders.VisualTests.StaticContent.Content.Pages
{
    public class PageProvider : ContentProviderBase
    {
        private readonly AssetModel _assets;

        public PageProvider(
            StaticStartPage entryPoint,
            AssetModel assets,
            IContentTypeRepository contentTypeRepository,
            IObjectInstanceCache objectInstanceCache)
            : base(entryPoint, 250000, contentTypeRepository, objectInstanceCache)
        {
            _assets = assets;
        }

        protected override void Initialize()
        {
            var narrowRenderSettings = new Dictionary<string, object> { { "data-epi-content-display-option", "narrow" } };
            var examplePages = Create<StandardPage>(
                Tree,
                "Example Pages");

            var teaserPage = Create<StandardPage>(
                examplePages, // Create as child to the Example page
                "Teasers",
                page =>
                {
                    page.PageImage = _assets.Image;
                    page.TeaserText = "Here are some examples of teasers in various sizes.";
                    // Add the teasers later, we need an example page that will be added to the Content Area first
                });
            var pageAsTeaser = Create<StandardPage>(
                teaserPage, // Add the page that should be used in the Content Area as a child to the newly created page.
                "Page as Teaser",
                page =>
                {
                    page.VisibleInMenu = false;
                    page.PageImage = _assets.Image;
                    page.TeaserText = "This is what it looks like when a teaser is added to a page.";
                });

            // Add the teasers as well as the page to the Main Content Area
            Update(teaserPage, page =>
            {
                var contentArea = new ContentArea();
                foreach (var teaser in _assets.Teasers)
                {
                    contentArea.Items.Add(new ContentAreaItem
                    {
                        ContentLink = teaser
                    });
                    contentArea.Items.Add(new ContentAreaItem
                    {
                        ContentLink = teaser,
                        RenderSettings = narrowRenderSettings
                    });
                }

                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = pageAsTeaser.ContentLink,
                    RenderSettings = narrowRenderSettings
                });
                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = pageAsTeaser.ContentLink
                });

                page.MainContentArea = contentArea;
            });

            var textFormatPage = Create<StandardPage>(
                examplePages, // Create as child to the Example page
                "Text format",
                page =>
                {
                    page.TeaserText = "Here are some examples of text in various formats.";

                    var mainBody = "<h2>Header 2</h2>" +
                        "<h3>Header 3</h3>" +
                        "<p>Paragraph with <strong>strong</strong> and <em>italic</em> text. Also a <strong><em>combination</em></strong>.</p>" +
                        "<ul>" +
                        "<li>Bullet 1</li>" +
                        "<li>Bullet 2</li>" +
                        "<li>Bullet 3</li>" +
                        "</ul>" +
                        "<ol>" +
                        "<li>Numbered 1</li>" +
                        "<li>Numbered 2</li>" +
                        "<li>Numbered 3</li>" +
                        "</ol>";
                    page.MainBody = new XhtmlString(mainBody);
                });

            Update(examplePages, page =>
            {
                var contentArea = new ContentArea();
                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = teaserPage.ContentLink,
                    RenderSettings = narrowRenderSettings
                });
                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = textFormatPage.ContentLink,
                    RenderSettings = narrowRenderSettings
                });
                page.MainContentArea = contentArea;
            });
        }
    }
}
