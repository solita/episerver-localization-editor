using EPiServer.Shell;
using Solita.LocalizationEditor.UI.Controllers;

namespace Solita.LocalizationEditor.UI.Common
{
    public static class ModuleUtil
    {
        public static string PathTo(string relativePath)
        {
            return Paths.ToResource(typeof (LocalizationEditorController), relativePath.TrimStart('~', '/'));
        }
    }
}