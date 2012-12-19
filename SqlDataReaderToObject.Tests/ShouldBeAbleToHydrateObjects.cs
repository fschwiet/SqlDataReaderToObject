using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NJasmine;
using Newtonsoft.Json;
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

            given("a table whose columns have all the types and a single row", () =>
            {
                var dto = new AllTheTypes()
                {
                    TheBinary = new byte[0]
                    //TheDate = new DateTime(2000,1,1),
                    //TheSmallDateTime = new DateTime(2000,1,1),
                    //TheDateTime = new DateTime(2000,1,1),
                    //TheDateTime2 = new DateTime(2000, 1, 1),
                    //TheDateTimeOffset = new DateTime(2000, 1, 1)
                };

                beforeAll(() =>
                {
                    var fields = typeof(AllTheTypes).GetFields();
                    var insertedFields = fields.Where(f => f.Name != "TheTimeStamp");

                    var tableExpression = new StringBuilder();
                    tableExpression.AppendLine("CREATE TABLE AllTheTypes(");

                    var insertExpression = new StringBuilder();
                    insertExpression.AppendLine("INSERT INTO AllTheTypes(");

                    var separator = "";
                    foreach (var field in fields)
                    {
                        var sqlType = field.Name.Substring("The".Length).ToLower();

                        tableExpression.AppendLine(string.Format("    {0}[{1}] [{2}]", separator, field.Name, sqlType));
                        separator = ", ";
                    }

                    tableExpression.AppendLine(")");

                    separator = "";
                    foreach (var field in insertedFields)
                    {
                        insertExpression.AppendLine(string.Format("     {0}{1}", separator, field.Name));
                        separator = ", ";
                    }

                    separator = "";
                    insertExpression.AppendLine(") VALUES (");
                    foreach (var field in insertedFields)
                    {
                        insertExpression.AppendLine(string.Format("     {0}@{1}", separator, field.Name));
                        separator = ", ";
                    }
                    insertExpression.AppendLine(")");

                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    foreach (var field in insertedFields)
                    {
                        var value = field.GetValue(dto);
                        parameters.Add("@" + field.Name, value);
                    }

                    Console.WriteLine(tableExpression.ToString());
                    Console.WriteLine(insertExpression.ToString());
                    Console.WriteLine(JsonConvert.SerializeObject(parameters));

                    database.RunNonQuery(tableExpression.ToString());
                    database.RunNonQuery(insertExpression.ToString(), parameters);
                });

                then("we should be able to read the row", () =>
                {
                    AllTheTypes result = null;

                    database.RunQuery(r => { result = new SqlMapper().ReadObjects<AllTheTypes>(r).Single(); });

                    var serializedResult = JsonConvert.SerializeObject(result);
                    var expectedResult = JsonConvert.SerializeObject(dto);
                    expect(() => serializedResult == expectedResult);
                });
            });
        }
    }
}
