namespace Solita.LanguageEditor.Definitions
{
    public class LocalizationDefinition
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DefaultValue { get; set; }

        public LocalizationDefinition(string key, string description, string category, string defaultValue)
        {
            Key = key;
            Description = description;
            Category = category;
            DefaultValue = defaultValue;
        }
    }
}