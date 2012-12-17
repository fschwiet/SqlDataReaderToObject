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
                using (var connection = database.GetConnection())
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"CREATE TABLE Foo([column1] [bigint] NOT NULL)";
                        command.ExecuteNonQuery();
                    }
                }

                using (var connection = database.GetConnection())
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(*) + 123 FROM Foo";
                        var result = (int)command.ExecuteScalar();
                        Assert.That(result, Is.EqualTo(123));
                    }
                }

                // leaked connections are ok
                database.GetConnection().Open();
                database.GetConnection();
            }
        }
    }
}
