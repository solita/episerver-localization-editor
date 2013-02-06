using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Solita.LanguageEditor.Definitions
{
    public class DefinitionsHelpers
    {
        public static IEnumerable<Type> GetCategoryTypes()
        {
            return from assembly in AppDomain.CurrentDomain.GetAssemblies()
                   from type in assembly.GetTypes()
                   let attribute = GetAttribute<LocalizationCategoryAttribute>(type)
                   where attribute != null
                   orderby attribute.Order
                   select type;
        }

        public static T GetAttribute<T>(MemberInfo type) where T : Attribute
        {
            return (T) Attribute.GetCustomAttribute(type, typeof (T));
        }

        public static IEnumerable<FieldInfo> GetLocalizationFields(IReflect type)
        {
            return from field in type.GetFields(BindingFlags.Static | BindingFlags.Public)
                   where GetAttribute<LocalizationAttribute>(field) != null
                   select field;
        }
    }
}