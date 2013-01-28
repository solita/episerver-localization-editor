using System.Collections.Generic;

namespace Solita.LanguageEditor.UI.Models
{
    public class LanguageEditorViewModel
    {
        public IList<string> Languages { get; set; }
        public IList<Translation> Translations { get; set; }

        public LanguageEditorViewModel()
        {
            Translations = new List<Translation>();
        }

        public Translation AddTranslation(string key, string description, string category, int order, string defaultValue)
        {
            var translation = new Translation { Key = key, Description = description, Category =  category, Order = order, DefaultValue = defaultValue};
            Translations.Add(translation);
            return translation;
        }

        public class Translation
        {
            public string Key { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public int Order { get; set; }
            public IDictionary<string, string> Translations { get; set; }
            public string DefaultValue { get; set; }

            public Translation()
            {
                Translations = new Dictionary<string, string>();
            }

            public void AddTranslation(string languageId, string value)
            {
                Translations.Add(languageId, value);
            }
        }
    }
}