using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solita.LocalizationEditor.Definitions;

namespace Solita.LocalizationEditor.Tests 
{

    /// <summary>
    /// Unit tests for <see cref="LocalizationHelpers"/>
    /// </summary>
    [TestClass]
    public class LocalizationHelpersTests 
    {

        private LocalizationDefinition[] GetLocalizationDefinitions(string categoryName)
        {
            return LocalizationHelpers.GetLocalizationDefinitions().Where(l => l.Category == categoryName).ToArray();
        }

        [TestMethod]
        public void GetLocalizationDefinitions_AllDefinitions()
        {
            
            var result = LocalizationHelpers.GetLocalizationDefinitions();

            Assert.IsNotNull(result, "result");
            Assert.AreEqual(5, result.Count, "Number of localization definitions");

        }

        [TestMethod]
        public void GetLocalizationDefinitions_AbsoluteKey()
        {
            
            var definitions = GetLocalizationDefinitions("Test");

            Assert.AreEqual(2, definitions.Length, "Number of definitions with absolute key");

            var first = definitions[0];
            Assert.AreEqual("First test value", first.Description, "first.Description");
            Assert.AreEqual("First", first.DefaultValue, "first.DefaultValue");
            Assert.AreEqual("Test", first.Category, "first.Category");
            Assert.AreEqual("/test/first", first.Key, "first.Key");

            var second = definitions[1];
            Assert.AreEqual("Second", second.Description, "second.Description");

        }

        [TestMethod]
        public void GetLocalizationDefinitions_PrefixKey()
        {
            
            // Get definitions from LocalizationCategoryWithPrefix
            var definitions = GetLocalizationDefinitions("Test with prefix");

            Assert.AreEqual(2, definitions.Length, "Number of definitions with prefix key");

            var first = definitions[0];
            Assert.AreEqual("First", first.DefaultValue, "first.DefaultValue");
            Assert.AreEqual("/withPrefix/prefixFirst", first.Key, "first.Key");

            var second = definitions[1];
            Assert.AreEqual("Second", second.DefaultValue, "second.DefaultValue");
            Assert.AreEqual("/withPrefix/prefixSecond", second.Key, "second.Key");

        }

        [TestMethod]
        public void GetLocalizationDefinitions_NoTrailingSlashInPrefix()
        {

            var definitions = GetLocalizationDefinitions("Test with no trailing slash in prefix");

            // Get definitions from LocalizationCategoryWithPrefix
            var first = definitions[0];
            Assert.AreEqual("First", first.DefaultValue, "first.DefaultValue");
            Assert.AreEqual("/withPrefix/prefixFirst", first.Key, "first.Key");            

        }

    }

}
