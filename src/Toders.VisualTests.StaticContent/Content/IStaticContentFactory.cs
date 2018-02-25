using System.Collections.Generic;

namespace Toders.VisualTests.StaticContent.Content
{
    public interface IStaticContentFactory
    {
        IContentNode Tree { get; }

        IEnumerable<IContentNode> AllContent { get; }
    }
}