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

        [TestMethod]
        public void GetLocalizationDefinitions_AbsoluteKey()
        {
            
            // Gets definitions from TestLocalizationCategory
            var result = LocalizationHelpers.GetLocalizationDefinitions();

            Assert.IsNotNull(result, "result");
            Assert.AreEqual(4, result.Count, "Number of localization definitions");

            var withAbsoluteKey = result.Where(t => t.Category == "Test").ToArray();

            Assert.AreEqual(2, withAbsoluteKey.Length, "Number of definitions with absolute key");

            var first = withAbsoluteKey[0];
            Assert.AreEqual("First test value", first.Description, "first.Description");
            Assert.AreEqual("First", first.DefaultValue, "first.DefaultValue");
            Assert.AreEqual("Test", first.Category, "first.Category");
            Assert.AreEqual("/test/first", first.Key, "first.Key");

            var second = withAbsoluteKey[1];
            Assert.AreEqual("Second", second.Description, "second.Description");

        }

        [TestMethod]
        public void GetLocalizationDefinitions_PrefixKey()
        {
            
            // Gets definitions from TestLocalizationCategory
            var result = LocalizationHelpers.GetLocalizationDefinitions();

            var definitions = result.Where(t => t.Category == "Test with prefix").ToArray();

            Assert.AreEqual(2, definitions.Length, "Number of definitions with prefix key");

            var first = definitions[0];
            Assert.AreEqual("First", first.DefaultValue, "first.DefaultValue");
            Assert.AreEqual("/withPrefix/prefixFirst", first.Key, "first.Key");

            var second = definitions[1];
            Assert.AreEqual("Second", second.DefaultValue, "second.DefaultValue");
            Assert.AreEqual("/withPrefix/prefixSecond", second.Key, "second.Key");

        }

    }

}
