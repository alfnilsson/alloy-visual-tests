using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Cache;
using EPiServer.Globalization;
using EPiServer.Web.Routing;

namespace Toders.VisualTests.StaticContent.Content
{
    public abstract class ContentProviderBase : ContentProvider
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IObjectInstanceCache _objectInstanceCache;

        //private TStaticContentFactory _staticContentFactory;
        private readonly List<IContentNode<IContentData>> _allContents = new List<IContentNode<IContentData>>();

        private int _nextId;

        public override string ProviderKey
        {
            get
            {
                return GetType().Name;
            }
        }

        protected ContentProviderBase(IContent entryPoint, int nextId, IContentTypeRepository contentTypeRepository, IObjectInstanceCache objectInstanceCache)
        {
            _nextId = nextId;
            _contentTypeRepository = contentTypeRepository;
            _objectInstanceCache = objectInstanceCache;

            this.Tree = new ContentNode<IContent>
            {
                Content = entryPoint
            };
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            IContentNode<IContentData> contentNode = AllContent.FirstOrDefault(x => x.ContentLink.Equals(contentLink));
            if (contentNode == null)
            {
                return null;
            }

            return contentNode.Content as IContent;
        }

        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageId, out bool languageSpecific)
        {
            languageSpecific = false;
            return AllContent
                .Where(x => x.Parent.ContentLink.CompareToIgnoreWorkID(contentLink))
                .Select(x => new GetChildrenReferenceResult
                {
                    ContentLink = x.ContentLink,
                    ModelType = x.Content.GetOriginalType()
                })
                .ToList();
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            this.Initialize();

            foreach (var content in this.AllContent)
            {
                MakeReadOnly(content);
            }
        }

        public IContentNode<IContent> Tree { get; }

        public IEnumerable<IContentNode<IContentData>> AllContent
        {
            get { return _allContents; }
        }

        protected abstract void Initialize();

        protected ContentNode<TContent> Create<TContent>(IContentNode<IContent> parentNode, string name, Action<TContent> populate = null)
            where TContent : class, IContentData
        {
            var parent = parentNode.Content;

            CultureInfo language;
            ILocalizable localizableParent = parent as ILocalizable;
            if (localizableParent != null && localizableParent.Language != null)
            {
                language = localizableParent.Language;
            }
            else
            {
                language = ContentLanguage.PreferredCulture;
            }

            IContent defaultContent = GetDefaultContent(
                parent,
                _contentTypeRepository.Load<TContent>().ID,
                new LanguageSelector(language.Name));

            defaultContent.Name = name;

            var contentLink = GetNextContentLink();
            defaultContent.ContentLink = contentLink;
            defaultContent.ContentGuid = GetOrCreateGuid(contentLink);

            DateTime date = DateTime.Now.AddDays(-7);

            var versionable = defaultContent as IVersionable;
            if (versionable != null)
            {
                versionable.StartPublish = date;
                versionable.Status = VersionStatus.Published;
                versionable.IsPendingPublish = false;
            }

            var changeTrackable = defaultContent as IChangeTrackable;
            if (changeTrackable != null)
            {
                changeTrackable.Changed = date;
                changeTrackable.Created = date;
                changeTrackable.Saved = date;

                changeTrackable.ChangedBy = "Static";
                changeTrackable.CreatedBy = "Static";
            }

            var routable = defaultContent as IRoutable;
            if (routable != null)
            {
                routable.RouteSegment = name.ToLower().Replace(" ", "-");
            }

            populate?.Invoke((TContent)defaultContent);

            var localizable = defaultContent as ILocalizable;
            if (localizable != null)
            {
                localizable.ExistingLanguages = new[] { localizable.MasterLanguage };
            }

            // Normally we make the content read only here, but we need to update it later

            var node = new ContentNode<TContent>
            {
                Content = (TContent)defaultContent,
                Parent = parentNode
            };

            parentNode.Children.Add(node);
            _allContents.Add(node);

            return node;
        }

        protected void Update<TContent>(IContentNode<TContent> node, Action<TContent> populate)
            where TContent : class, IContentData
        {
            var content = node.Content;
            if (content == null)
            {
                return;
            }

            populate(content);
        }
        private static void MakeReadOnly(IContentNode<IContentData> content)
        {
            var readOnly = content as IReadOnly;
            if (readOnly != null)
            {
                readOnly.MakeReadOnly();
            }
        }

        private Guid GetOrCreateGuid(ContentReference contentLink)
        {
            string cacheKey = $"StaticContentGuid_{contentLink}";
            object cachedObject = _objectInstanceCache.Get(cacheKey);
            if (cachedObject != null)
            {
                return (Guid)cachedObject;
            }

            var guid = Guid.NewGuid();
            _objectInstanceCache.Insert(cacheKey, guid, CacheEvictionPolicy.Empty);
            return guid;
        }

        private ContentReference GetNextContentLink()
        {
            return new ContentReference(_nextId++)
            {
                ProviderName = Name
            };
        }
    }
}