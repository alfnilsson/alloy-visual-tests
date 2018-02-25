using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;

namespace Toders.VisualTests.StaticContent.Content
{
    public abstract class StaticContentFactoryBase<TEntryPoint> : IStaticContentFactory
        where TEntryPoint : IContentData
    {
        private int _nextId;
        private readonly ContentProvider _contentProvider;
        private readonly IContentLoader _contentLoader;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly List<IContentNode> _allContents = new List<IContentNode>();

        protected StaticContentFactoryBase(
            TEntryPoint entryPoint,
            int nextId,
            ContentProvider contentProvider,
            IContentLoader contentLoader,
            IContentTypeRepository contentTypeRepository)
        {
            _nextId = nextId;
            _contentProvider = contentProvider;
            _contentLoader = contentLoader;
            _contentTypeRepository = contentTypeRepository;

            this.Tree = new ContentNode<IContentData>
            {
                Content = entryPoint
            };
        }

        public IContentNode Tree { get; }

        public IEnumerable<IContentNode> AllContent
        {
            get { return _allContents; }
        }

        protected IContentNode Create<TContent>(IContentNode<IContentData> parentNode, string name, Action<TContent> populate = null)
            where TContent : class, IContentData
        {
            var parent = parentNode.Content;

            CultureInfo language;
            ILocalizable localizable = parent as ILocalizable;
            if (localizable != null && localizable.Language != null)
            {
                language = localizable.Language;
            }
            else
            {
                language = ContentLanguage.PreferredCulture;
            }

            IContent defaultContent = _contentProvider.GetDefaultContent(
                _contentLoader.Get<IContent>(parentNode.ContentLink),
                _contentTypeRepository.Load<TContent>().ID,
                new LanguageSelector(language.Name));

            defaultContent.Name = name;
            defaultContent.ContentLink = GetNextContentLink();

            populate?.Invoke((TContent)defaultContent);

            IContentNode node = new ContentNode<TContent>
            {
                Content = (TContent)defaultContent,
                Parent = parentNode
            };

            parentNode.Children.Add(node);
            _allContents.Add(node);

            return node;
        }

        private ContentReference GetNextContentLink()
        {
            return new ContentReference(_nextId++)
            {
                ProviderName = _contentProvider.Name
            };
        }
    }
}