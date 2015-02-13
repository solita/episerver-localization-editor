using System.Collections.Generic;
using System.Linq;

namespace Solita.LocalizationEditor.UI.Models
{
    public class CategoryList: List<CategoryList.Category>
    {
        public Translation AddTranslation(string key, string description, string categoryName, string defaultValue)
        {
            var translation = new Translation { Key = key, Description = description, DefaultValue = defaultValue };

            var category = this.FirstOrDefault(cat => cat.Name == categoryName);
            if (category == null)
            {
                category = new Category { Name = categoryName };
                Add(category);
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