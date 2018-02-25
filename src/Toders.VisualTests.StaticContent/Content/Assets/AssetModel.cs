using System.Collections.Generic;
using EPiServer.Core;

namespace Toders.VisualTests.StaticContent.Content.Assets
{
    public class AssetModel
    {
        public ContentReference Image { get; set; }

        public List<ContentReference> Teasers { get; set; } = new List<ContentReference>();

    }
}