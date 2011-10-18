using DynamicPad;
using NUnit.Framework;

namespace DynamicPadTests
{
    [TestFixture]
    public class ConnectionStringRepositoryTests
    {
        [Test]
        public void Add_WhenConnectionStringNameIsNull_ThrowsArgumentNullException()
        {
            var sut = new ConnectionStringRepository();
            sut.Add(null, "");


        }
    }
}