using System.Collections.Generic;
using System.Linq;

namespace Solita.LanguageEditor.UI.Models
{
    public class LanguageEditorViewModel
    {
        public IList<string> Languages { get; set; }
        public CategoryList Categories { get; set; }
        public bool DevelopmentMode { get; set; }
        public IList<XmlVersionInfo> Versions { get; set; }
        
        public LanguageEditorViewModel()
        {
            Categories = new CategoryList();
        }
    }
}