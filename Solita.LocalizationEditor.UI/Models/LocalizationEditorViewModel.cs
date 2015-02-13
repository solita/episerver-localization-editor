using System.Collections.Generic;
using System.Linq;

namespace Solita.LocalizationEditor.UI.Models
{
    public class LocalizationEditorViewModel
    {
        public IList<string> Languages { get; set; }
        public CategoryList Categories { get; set; }
        public bool PowertoolsMode { get; set; }
        public IList<XmlVersionInfo> Versions { get; set; }
        
        public LocalizationEditorViewModel()
        {
            Categories = new CategoryList();
        }
    }
}