using System.Collections.Generic;
using System.Linq;

namespace Solita.LanguageEditor.UI.Definitions
{
    public class LocalizationHelpers
    {
        public static IList<LocalizationDefinition> GetLocalizationDefinitions()
        {
            return
                (from categoryType in LocalizationAttributeHelpers.FindCategoryTypes()
                 let categoryAttribute = LocalizationAttributeHelpers.GetCategoryAttribute(categoryType)
                 from field in LocalizationAttributeHelpers.FindLocalizationFields(categoryType)
                 let attribute = LocalizationAttributeHelpers.GetLocalizationAttribute(field)
                 let key = (string)field.GetValue(null)
                 select new LocalizationDefinition(key, attribute.Description, categoryAttribute.Name, attribute.DefaultValue))
                    .ToList();
        }
    }
}
