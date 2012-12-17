using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SqlDataReaderToObject.Tests
{
    [TestFixture]
    public class TempDatabaseTest
    {
        [Test]
        public void ShouldBeAbleToCreateTestDatabase()
        {
            using (var database = TempDatabase.Create())
            {
                database.RunNonQuery(@"CREATE TABLE Foo([column1] [bigint] NOT NULL)");

                int result = (int)database.RunScalar(@"SELECT COUNT(*) + 123 FROM Foo");

                Assert.That(result, Is.EqualTo(123));

                // leaked connections are ok
                database.GetConnection().Open();
                database.GetConnection();
            }
        }
    }
}
