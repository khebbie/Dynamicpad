using System;
using System.Collections.Generic;
using System.Linq;
using DynamicPad;
using NUnit.Framework;

namespace DynamicPadTests
{
    [TestFixture]
    public class CfgFileSerializerTests
    {
        private CfgFileSerializer _sut;

        [SetUp]
        public void BeforeEach()
        {
            _sut = new CfgFileSerializer();
        }

        [Test]
        public void Serialize_Null_ReturnsEmptyDictionary()
        {
            Dictionary<string, string> cfg = _sut.Serialize(null);

            Assert.AreEqual(0, cfg.Count);
        }

        [Test]
        public void Serialize_NoComma_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.Serialize("test"));
        }

        [Test]
        public void Serialize_LineTwoIsEmpty_DoesNotThrow()
        {
            const string name = "name";
            const string connectionstring = "connectionstring";
            var fileContent = string.Format("{0}, {1}", name, connectionstring);
            fileContent += "\n";

            Assert.DoesNotThrow(() => _sut.Serialize(fileContent));
        }

        [Test]
        public void Serialize_TwoStringsSeperatedByComma_DictionaryWithStrings()
        {
            const string name = "name";
            const string connectionstring = "connectionstring";
            
            var result = _sut.Serialize( string.Format("{0}, {1}", name, connectionstring));

            var cfg = result.First();

            Assert.AreEqual(name, cfg.Key);
            Assert.AreEqual(connectionstring, cfg.Value);
        }

        [Test]
        public void Deserialize_EmptyDictionary_EmptyString()
        {
            var cfg = new Dictionary<string, string>();
            string result = _sut.Deserialize(cfg);
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void Deserialize_OneEntryDictionary_OneLineCommaSeperated()
        {
            const string name = "name";
            const string connectionstring = "connectionstring";
            var cfg = new Dictionary<string, string> {{name, connectionstring}};
            string result = _sut.Deserialize(cfg);

            string expectedResult = string.Format("{0}, {1}\n", name, connectionstring);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Deserialize_ThreeEntiesDictionary_ThreeLinesCommaSeperated()
        {
            const string name = "name";
            const string connectionstring = "connectionstring";
            var cfg = new Dictionary<string, string>
                          {
                              {name+"1", connectionstring},
                              {name+"2", connectionstring},
                              {name+"3", connectionstring}
                          };
            string result = _sut.Deserialize(cfg);

            var expectedResult = string.Format("{0}1, {1}", name, connectionstring) + "\n" + string.Format("{0}2, {1}", name, connectionstring) + "\n" + string.Format("{0}3, {1}\n", name, connectionstring);
            Assert.AreEqual(expectedResult, result);
        }
    }
}