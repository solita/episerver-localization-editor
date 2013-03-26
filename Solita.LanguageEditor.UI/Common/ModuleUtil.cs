using EPiServer.Shell;
using Solita.LanguageEditor.UI.Controllers;

namespace Solita.LanguageEditor.UI.Common
{
    public static class ModuleUtil
    {
        public static string PathTo(string relativePath)
        {
            return Paths.ToResource(typeof (LanguageEditorController), relativePath.TrimStart('~', '/'));
        }
    }
}