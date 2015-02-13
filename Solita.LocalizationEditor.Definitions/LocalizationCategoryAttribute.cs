using System;

namespace Solita.LocalizationEditor.Definitions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LocalizationCategoryAttribute : Attribute
    {
        public string Name { get;  private set; }
        public int Order { get; private set; }

        public LocalizationCategoryAttribute(string name, int order)
        {
            Name = name;
            Order = order;
        }
    }
}