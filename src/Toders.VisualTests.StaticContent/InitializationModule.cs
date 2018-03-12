using System;
using System.Collections.Specialized;
using System.Linq;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.Cache;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Toders.VisualTests.StaticContent.Content.Assets;
using Toders.VisualTests.StaticContent.Content.Pages;
using Toders.VisualTests.StaticContent.Models;

namespace Toders.VisualTests.StaticContent
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class InitializationModule : IInitializableModule
    {
        private const string StaticWebsiteUrl = "http://localhost:2335"; // Change this to your own URL

        public void Initialize(InitializationEngine context)
        {
            var contentProviderManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();

            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var startPage = contentRepository.GetChildren<StaticStartPage>(ContentReference.RootPage).FirstOrDefault();
            if (startPage == null)
            {
                return;
            }

            StaticAssetsRootFolder assetsFolder = EnsureAssetsFolder(contentRepository);

            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var objectInstanceCache = ServiceLocator.Current.GetInstance<IObjectInstanceCache>();
            var blobFactory = ServiceLocator.Current.GetInstance<IBlobFactory>();

            AssetProvider assetProvider = new AssetProvider(assetsFolder, contentTypeRepository, objectInstanceCache, blobFactory);
            SetupProvider(assetProvider, assetsFolder.ContentLink, contentProviderManager);

            var pageProvider = new PageProvider(startPage, assetProvider.Assets, contentTypeRepository, objectInstanceCache);
            SetupProvider(pageProvider, startPage.PageLink, contentProviderManager);

            var siteDefinitionRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
            EnsureSiteDefinition(siteDefinitionRepository, startPage.ContentLink, assetsFolder.ContentLink);
        }

        private static StaticAssetsRootFolder EnsureAssetsFolder(IContentRepository contentRepository)
        {
            var assetsFolder = contentRepository.GetChildren<StaticAssetsRootFolder>(ContentReference.RootPage).FirstOrDefault();
            if (assetsFolder == null)
            {
                assetsFolder = contentRepository.GetDefault<StaticAssetsRootFolder>(ContentReference.RootPage);
                assetsFolder.Name = "Static Assets";
                contentRepository.Save(assetsFolder, AccessLevel.NoAccess);
            }

            return assetsFolder;
        }

        private static void SetupProvider<TProvider>(TProvider provider, ContentReference entryPoint, IContentProviderManager contentProviderManager)
            where TProvider : ContentProvider
        {
            NameValueCollection config = new NameValueCollection
            {
                { ContentProviderElement.EntryPointString, entryPoint.ToString() }
            };

            provider.Initialize(provider.Name, config);

            contentProviderManager.ProviderMap.AddProvider(provider);
        }

        private static void EnsureSiteDefinition(ISiteDefinitionRepository siteDefinitionRepository,
            ContentReference startPage, ContentReference assetsFolder)
        {
            var siteDefinition = siteDefinitionRepository.List()
                .FirstOrDefault(x => x.StartPage.CompareToIgnoreWorkID(startPage));

            if (siteDefinition != null)
            {
                return;
            }

            siteDefinition = new SiteDefinition
            {
                Name = "Static Website",
                StartPage = startPage,
                SiteAssetsRoot = assetsFolder,
                SiteUrl = new Uri(StaticWebsiteUrl) // To do: Should come from configuration?
            };

            siteDefinitionRepository.Save(siteDefinition);
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }
    }
}
