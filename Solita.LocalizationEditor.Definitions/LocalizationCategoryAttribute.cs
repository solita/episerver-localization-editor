using System;

namespace Solita.LocalizationEditor.Definitions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LocalizationCategoryAttribute : Attribute
    {
        public string Name { get;  private set; }
        public int Order { get; private set; }

        /// <summary>
        /// Prefix applied to keys of all definitions. Starting and trailing slash are optional.
        /// </summary>
        public string KeyPrefix { get; set; }

        public LocalizationCategoryAttribute(string name, int order, string keyPrefix = null)
        {
            Name = name;
            Order = order;
            KeyPrefix = keyPrefix;
        }
    }
}