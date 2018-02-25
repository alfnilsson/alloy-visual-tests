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
                examplePages,
                "Teasers",
                page =>
                {
                    page.PageImage = _assets.Image;
                    page.TeaserText = "Here are some examples of teasers in various sizes.";
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

                    page.MainContentArea = contentArea;
                });
            var pageAsTeaser = Create<StandardPage>(
                teaserPage,
                "Page as Teaser",
                page =>
                {
                    page.VisibleInMenu = false;
                    page.PageImage = _assets.Image;
                    page.TeaserText = "This is what it looks like when a teaser is added to a page.";
                });

            Update(teaserPage, page =>
            {
                var contentArea = page.MainContentArea;
                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = pageAsTeaser.ContentLink,
                    RenderSettings = narrowRenderSettings
                });
                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = pageAsTeaser.ContentLink
                });
            });

            Update(examplePages, page =>
            {
                var contentArea = new ContentArea();
                contentArea.Items.Add(new ContentAreaItem
                {
                    ContentLink = teaserPage.ContentLink,
                    RenderSettings = narrowRenderSettings
                });
                page.MainContentArea = contentArea;
            });
        }
    }
}
