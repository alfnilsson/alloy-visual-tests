using System.Collections.Generic;
using EPiServer.Core;

namespace Toders.VisualTests.StaticContent.Content
{
    public class ContentNode<TContent> : IContentNode<TContent>
        where TContent : class, IContentData

    {
        public ContentNode()
        {
            this.Children = new List<IContentNode>();
        }

        public ContentReference ContentLink
        {
            get { return ((IContent)this.Content).ContentLink; }
        }

        public TContent Content { get; set; }

        public List<IContentNode> Children { get; set; }

        public IContentNode Parent { get; set; }
    }

    public interface IContentNode<out TContent> : IContentNode
        where TContent : class, IContentData
    {
        TContent Content { get; }

    }

    public interface IContentNode
    {
        ContentReference ContentLink { get; }
        List<IContentNode> Children { get; }
        IContentNode Parent { get; set; }
    }
}