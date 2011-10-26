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
            _sut.Clear();
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

        [Test]
        public void AddGet_CanAddAndRetrieve()
        {
            const string myConnectionString = "MyConnectionString";
            _sut.Add(myConnectionString, ConnectionString);

            string connectionString = _sut.Get(myConnectionString);
            Assert.AreEqual(connectionString, ConnectionString);
        }

        [Test]
        public void AddAlreadyExistingConnectionString_ThrowsArgumentException()
        {
            const string myConnectionString = "MyConnectionString";
            _sut.Add(myConnectionString, ConnectionString);
            Assert.Throws<ArgumentException>(() =>_sut.Add(myConnectionString, ConnectionString));
        }

        [Test]
        public void Add_ConnectionStringAlreadyExists_Throws()
        {
            const string myConnectionString = "MyConnectionString";
            _sut.Add(myConnectionString, ConnectionString);

            Assert.Throws<ArgumentException>(() => _sut.Add(myConnectionString, ConnectionString));
        }

        [Test]
        public void Add_ThenNew_ReadsFileCorrectly()
        {
            const string myConnectionString = "MyConnectionString";
            _sut.Add(myConnectionString, ConnectionString);

            _sut = new ConnectionStringRepository();

            var s = _sut.Get(myConnectionString);

            Assert.AreEqual(ConnectionString, s);
        }

        [Test]
        public void Add_ThenNew2Strings_ReadsFileCorrectly()
        {
            const string myConnectionString = "MyConnectionString";
            _sut.Add(myConnectionString, ConnectionString);
            _sut.Add(myConnectionString+2, ConnectionString);

            _sut = new ConnectionStringRepository();

            var s = _sut.Get(myConnectionString);
            var s2 = _sut.Get(myConnectionString);

            Assert.AreEqual(ConnectionString, s);
            Assert.AreEqual(ConnectionString+2, s2);
        }
    }
}