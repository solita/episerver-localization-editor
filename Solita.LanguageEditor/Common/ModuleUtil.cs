using EPiServer.Shell;
using Solita.LanguageEditor.Controllers;

namespace Solita.LanguageEditor.Common
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