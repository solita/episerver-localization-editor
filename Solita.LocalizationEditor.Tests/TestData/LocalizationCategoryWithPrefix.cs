using Solita.LocalizationEditor.Definitions;

namespace Solita.LocalizationEditor.Tests.TestData
{

    [LocalizationCategory("Test with prefix", 1, KeyPrefix = "/withPrefix/")]
    public class LocalizationCategoryWithPrefix
    {

        // Key will be /withPrefix/prefixFirst
        [Localization(Description = "First test value", DefaultValue = "First")]
        public const string PrefixFirst = "prefixFirst";

        [Localization(Description = "Second", DefaultValue = "Second")]
        public const string PrefixSecond = "prefixSecond";

    }

}
