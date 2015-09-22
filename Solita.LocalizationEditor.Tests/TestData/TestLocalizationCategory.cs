using Solita.LocalizationEditor.Definitions;

namespace Solita.LocalizationEditor.Tests.TestData 
{

    /// <summary>
    /// Localization category test
    /// </summary>
    [LocalizationCategory("Test", 1)]
    public class TestLocalizationCategory 
    {

        [Localization(Description = "First test value", DefaultValue = "First")]
        public const string First = "/test/first";

        [Localization(Description = "Second", DefaultValue = "Second")]
        public const string Second = "/test/second";

        // Property without LocalizationAttribute is ignored
        public const string Third = "/test/third";

    }

}
