using System;
using System.Linq;
using EPiServer.Events.Clients;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using log4net;
using System.Collections.Specialized;

namespace Solita.LocalizationEditor.UI
{
    [InitializableModule]
    //A dependency to EPiServer CMS initialization is needed to be able to use a VPP
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))] 
    public class LocalizationProviderInitiator : IInitializableModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LocalizationProviderInitiator));
        public const string ProviderName = "SolitaVirtualPathLocalizationProvider";

        public void Initialize(InitializationEngine context)
        {
            //Casts the current LocalizationService to a ProviderBasedLocalizationService to get access to the current list of providers
            var localizationService = context.Locate.Advanced.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            if (localizationService == null)
            {
                Log.Error("LocalizationService unavailable");
                return;
            }
            
            AddProvider(localizationService);
            Event.Get(LocalizationsUpdatedEventHandler.EventId).Raised += LocalizationsUpdatedEventHandler.HandleUpdate;
        }

        private static void AddProvider(ProviderBasedLocalizationService service)
        {
            try
            {
                var localizationContentProvider = new LocalizationContentProvider();
                localizationContentProvider.Initialize(ProviderName, new NameValueCollection());

                service.Providers.Insert(0, localizationContentProvider);

                Log.Debug("LocalizationProvider added");
            }
            catch (Exception e)
            {
                Log.Error("Failed to add custom localization provider.", e);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Casts the current LocalizationService to a ProviderBasedLocalizationService to get access to the current list of providers
            var localizationService = context.Locate.Advanced.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            if (localizationService == null)
            {
                Log.Error("LocalizationService unavailable");
                return;
            }

            RemoveProvider(localizationService);
            Event.Get(LocalizationsUpdatedEventHandler.EventId).Raised -= LocalizationsUpdatedEventHandler.HandleUpdate;
        }

        private static void RemoveProvider(ProviderBasedLocalizationService service)
        {
            try
            {
                //Gets any provider that has the same name as the one initialized.
                var localizationProvider = service.Providers.FirstOrDefault(p => p.Name.Equals(ProviderName, StringComparison.Ordinal));
                if (localizationProvider != null)
                {
                    //If found, remove it.
                    service.Providers.Remove(localizationProvider);
                    Log.Debug("LocalizationProvider remove");
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to remove custom localization provider.", e);
            }
        }
        
        public static void ReInitProvider()
        {
            var service = ServiceLocator.Current.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            if (service == null)
            {
                Log.Error("LocalizationService unavailable");
                return;
            }

            RemoveProvider(service);
            AddProvider(service);
        }

        public void Preload(string[] parameters) { }
    }
}