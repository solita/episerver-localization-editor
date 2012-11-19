using System.Web.UI.WebControls;
using EPiServer.PlugIn;

namespace Solita.LanguageEditor.Common
{
    [GuiPlugIn]
    public class Settings
    {
        public static Settings AutoPopulated
        {
            get
            {
                var settings = new Settings();
                PlugInSettings.AutoPopulate(settings);
                return settings;
            }
        }

        [PlugInProperty(Description = "Virtual path to lang folder",
            AdminControl = typeof (TextBox),
            AdminControlValue = "Text")]
        public string LangFolderVirtualPath { get; set; }
    }
}