using System.Globalization;
using System.Resources;
using System.Threading;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Solita.LanguageEditor.UI.Lang {

    // This is a stub of resx-file code that provides property CmsMenuTitle. 
    // Resource files are the only way to provide localized names to episerver cms menu view.
    // But resource files are worst than death. So lets try to make this less fucked up (imo).
    internal class CmsMenuTitleProvider {
        internal static string CmsMenuTitle
        {
            get
            {
                const string key = "/languageeditor/title";
                var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();
                var languageId = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                string value;

                if (!localizationService.TryGetStringByCulture(key, new CultureInfo(languageId), out value))
                {
                    value = localizationService.GetStringByCulture(key, new CultureInfo("en"));
                }

                return value;
            }
        }
    }
}
