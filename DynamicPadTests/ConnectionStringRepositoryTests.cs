using System;
using System.IO;
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
            EnsureCfgFileDoesNotExist();
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

        [Test]
        public void Add_FileDoesNotExits_AddsFile()
        {
            

            const string myConnectionString = "MyConnectionString";
            _sut.Add(myConnectionString, ConnectionString);

            Assert.True(new FileInfo(CfgFileUtility.GetPathToFile()).Exists);
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
        public void Add_ConnectionStringAlreadyExists_Throws()
        {
            const string myConnectionString = "MyConnectionString";
            _sut.Add(myConnectionString, ConnectionString);

            Assert.Throws<ArgumentException>(() => _sut.Add(myConnectionString, ConnectionString));
        }

        private static void EnsureCfgFileDoesNotExist()
        {
            var pathToDirectory = CfgFileUtility.GetCfgDirectory();

            var cfgDirectory = new DirectoryInfo(pathToDirectory);
            if (!cfgDirectory.Exists)
                cfgDirectory.Create();

            var pathToFile = CfgFileUtility.GetPathToFile();
            var cfgFile = new FileInfo(pathToFile);
            if (cfgFile.Exists)
                cfgFile.Delete();
        }
    }
}