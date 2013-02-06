using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solita.LanguageEditor.Definitions
{
    public class ValidityTests
    {
        public static IEnumerable<string> GetDuplicateLocalizationKeys()
        {
            var duplicates =
                from category in DefinitionsHelpers.GetCategoryTypes()
                from localization in DefinitionsHelpers.GetLocalizationFields(category)
                group localization by localization.GetValue(null).ToString()
                into groups
                where groups.Count() > 1
                select groups.Key;

            return duplicates;
        }
    }
}
