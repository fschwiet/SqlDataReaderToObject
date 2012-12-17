using System;
using System.Collections.Generic;
using System.Data;
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
                    database.RunNonQuery("CREATE TABLE Foo([column1] [bigint] NOT NULL)");
                    database.RunNonQuery("INSERT INTO Foo (column1) Values (123)");
                });

                then("we should be able to read a row", () =>
                {
                    Foo result = null;

                    database.RunQuery(r => { result = new SqlMapper().ReadObjects<Foo>(r).Single(); });

                    expect(() => result.column1 == 123);
                });
            });
        }
    }
}
