using Solita.LocalizationEditor.Definitions;

namespace Solita.LocalizationEditor.Tests.TestData
{

    [LocalizationCategory("Test with no trailing slash in prefix", 1, KeyPrefix = "/withPrefix")]
    public class LocalizationCategoryWithPrefixNoTrailingSlash
    {

        // Key will be /withPrefix/prefixFirst
        [Localization(Description = "First test value", DefaultValue = "First")]
        public const string PrefixFirst = "prefixFirst";

    }

}
