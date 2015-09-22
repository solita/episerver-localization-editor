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
        public void GetLocalizationDefinitions()
        {
            
            // Gets definitions from TestLocalizationCategory
            var result = LocalizationHelpers.GetLocalizationDefinitions();

            Assert.IsNotNull(result, "result");
            Assert.AreEqual(2, result.Count, "Number of localization definitions");

            var first = result[0];
            Assert.AreEqual("First test value", first.Description, "first.Description");
            Assert.AreEqual("First", first.DefaultValue, "first.DefaultValue");
            Assert.AreEqual("Test", first.Category, "first.Category");
            Assert.AreEqual("/test/first", first.Key, "first.Key");

            var second = result[1];
            Assert.AreEqual("Second", second.Description, "second.Description");

        }

    }

}
