using System;
using System.Linq;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.XmlResources;
using EPiServer.Web.Hosting;
using Solita.LanguageEditor.Common;

namespace Solita.LanguageEditor
{
    [InitializableModule]
    //A dependency to EPiServer CMS initialization is needed to be able to use a VPP
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))] 
    public class LocalizationProviderInitiator : IInitializableModule
    {
        public const string ProviderName = "SolitaVirtualPathLocalizationProvider";

        public void Initialize(InitializationEngine context)
        {
            //Casts the current LocalizationService to a ProviderBasedLocalizationService to get access to the current list of providers
            var localizationService = context.Locate.Advanced.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            if (localizationService != null)
            {
                AddProvider(localizationService);
            }
        }

        private static void AddProvider(ProviderBasedLocalizationService service)
        {
            var langFolderVirtualPath = Settings.AutoPopulated.LangFolderVirtualPath;
            if(string.IsNullOrEmpty(langFolderVirtualPath))
            {
                return;
            }

            var localizationProviderInitializer = new VirtualPathXmlLocalizationProviderInitializer(GenericHostingEnvironment.VirtualPathProvider);
            //a VPP with the path below must be registered in the sites configuration.
            var localizationProvider = localizationProviderInitializer.GetInitializedProvider(langFolderVirtualPath, ProviderName);
            //Inserts the provider first in the provider list so that it is prioritized over default providers.
            service.Providers.Insert(0, localizationProvider);
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Casts the current LocalizationService to a ProviderBasedLocalizationService to get access to the current list of providers
            var localizationService = context.Locate.Advanced.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            if (localizationService != null)
            {
                RemoveProvider(localizationService);
            }
        }

        private static void RemoveProvider(ProviderBasedLocalizationService service)
        {
            //Gets any provider that has the same name as the one initialized.
            var localizationProvider = service.Providers.FirstOrDefault(p => p.Name.Equals(ProviderName, StringComparison.Ordinal));
            if (localizationProvider != null)
            {
                //If found, remove it.
                service.Providers.Remove(localizationProvider);
            }
        }


        public static void ReInitProvider()
        {
            var service = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            RemoveProvider(service);
            AddProvider(service);
        }

        public void Preload(string[] parameters) { }
    }
}