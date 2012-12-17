using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJasmine;
using SqlDataReaderToObject.Tests.DTOs;

namespace SqlDataReaderToObject.Tests
{
    public class ShouldBeAbleToHydrateObjects : GivenWhenThenFixture
    {
        public override void Specify()
        {
            var database = beforeAll(() => TempDatabase.Create());

            given("a basic table with a row", () =>
            {
                beforeAll(() =>
                {
                    using (var connection = database.GetConnection())
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "CREATE TABLE Foo([column1] [bigint] NOT NULL)";
                            command.ExecuteNonQuery();
                        }
                    }

                    using (var connection = database.GetConnection())
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "INSERT INTO Foo (column1) Values (123)";
                            command.ExecuteNonQuery();
                        }
                    }
                });

                then("we should be able to read a row", () =>
                {
                    Foo result;

                    using (var connection = database.GetConnection())
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT * FROM Foo";

                            using (var reader = command.ExecuteReader())
                            {
                                result = new SqlMapper().ReadObjects<Foo>(reader).Single();
                            }
                        }
                    }

                    expect(() => result.column1 == 123);
                });
            });
        }
    }
}
