using System;
using DynamicPad;
using NUnit.Framework;

namespace DynamicPadTests
{
    [TestFixture]
    public class ConnectionStringRepositoryTests
    {
        const string ConnectionString = @"Data Source=.\SQLExpress;Integrated Security=true; ;initial catalog=MassiveTest;";
        private ConnectionStringRepository _sut;

        [SetUp]
        public void BeforeEach()
        {
            _sut = new ConnectionStringRepository();
        }

        [Test]
        public void Add_ConnectionStringNameIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Add(null, ConnectionString));
        }

        [Test]
        public void Add_ConnectionStringIsNull_ThrowsArgumentNullException()
        {
            const string myConnectionString = "MyConnectionString";
            Assert.Throws<ArgumentNullException>(() => _sut.Add(myConnectionString, null));
        }
    }
}