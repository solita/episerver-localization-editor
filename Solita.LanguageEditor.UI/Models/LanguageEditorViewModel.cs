using System.Collections.Generic;
using System.Linq;

namespace Solita.LanguageEditor.UI.Models
{
    public class LanguageEditorViewModel
    {
        public IList<string> Languages { get; set; }
        public IList<Category> Categories { get; set; }
        
        public LanguageEditorViewModel()
        {
            Categories = new List<Category>();
        }

        public Translation AddTranslation(string key, string description, string categoryName, string defaultValue)
        {
            var translation = new Translation { Key = key, Description = description, DefaultValue = defaultValue };

            var category = Categories.FirstOrDefault(cat => cat.Name == categoryName);
            if (category == null)
            {
                category = new Category {Name = categoryName};
                Categories.Add(category);
            }
            
            category.Translations.Add(translation);
            return translation;
        }

        public class Category
        {
            public string Name { get; set; }
            public IList<Translation> Translations { get; set; }

            public Category()
            {
                Translations = new List<Translation>();
            }
        }

        public class Translation
        {
            public string Key { get; set; }
            public string Description { get; set; }
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