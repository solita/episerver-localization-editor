using EPiServer.Shell;
using Solita.LanguageEditor.UI.Controllers;

namespace Solita.LanguageEditor.UI.Common
{
    public static class ModuleUtil
    {
        public static string PathTo(string relativePath)
        {
            if (relativePath.StartsWith("~"))
                relativePath = relativePath.Substring(1);

            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);

            return Paths.ToResource(typeof(LanguageEditorController), relativePath);
        }
    }
}