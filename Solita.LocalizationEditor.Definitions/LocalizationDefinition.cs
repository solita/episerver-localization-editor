namespace Solita.LocalizationEditor.Definitions
{
    public class LocalizationDefinition
    {

        private string BuildKey(string key, string categoryPrefix)
        {
            
            if (string.IsNullOrEmpty(categoryPrefix) || string.IsNullOrEmpty(key))
                return key;

            if (!categoryPrefix.EndsWith("/") && !key.StartsWith("/"))
                return string.Format("{0}/{1}", categoryPrefix, key);

            return categoryPrefix + key;

        }

        public string Key { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string DefaultValue { get; set; }

        public LocalizationDefinition(string key, string description, string category, string defaultValue, string categoryPrefix)
        {
            Key = BuildKey(key, categoryPrefix);
            Description = description;
            Category = category;
            DefaultValue = defaultValue;
        }
    }
}