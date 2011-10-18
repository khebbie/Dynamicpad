using System;
using System.Collections.Generic;
using System.Linq;
using DynamicPad;
using NUnit.Framework;

namespace DynamicPadTests
{
    [TestFixture]
    public class CfgFileParserTests
    {
        private CfgFileParser _sut;

        [SetUp]
        public void BeforeEach()
        {
            _sut = new CfgFileParser();
        }

        [Test]
        public void Parse_Null_ReturnsEmptyDictionary()
        {
            Dictionary<string, string> cfg = _sut.Parse(null);

            Assert.AreEqual(0, cfg.Count);
        }

        [Test]
        public void Parse_NoComma_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _sut.Parse("test"));
        }

        [Test]
        public void Parse_TwoStringsSeperatedByComma_DictionaryWithStrings()
        {
            const string name = "name";
            const string connectionstring = "connectionstring";
            
            var result = _sut.Parse( string.Format("{0}, {1}", name, connectionstring));

            var cfg = result.First();

            Assert.AreEqual(name, cfg.Key);
            Assert.AreEqual(connectionstring, cfg.Value);
        }
    }
}