using Solita.LocalizationEditor.Definitions;

namespace Solita.LocalizationEditor.Tests.TestData 
{

    [LocalizationCategory("Test", 1)]
    public class TestLocalizationCategory 
    {

        [Localization(Description = "First", DefaultValue = "First")]
        public const string First = "/test/first";

        [Localization(Description = "Second", DefaultValue = "Second")]
        public const string Second = "/test/second";

        // Property without LocalizationAttribute is ignored
        public const string Third = "/test/third";

    }

}
