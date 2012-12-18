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
                var dto = new AllTheTypes();

                beforeAll(() =>
                {
                    var tableExpression = new StringBuilder();
                    tableExpression.AppendLine("CREATE TABLE AllTheTypes(");

                    var insertExpression = new StringBuilder();
                    insertExpression.AppendLine("INSERT INTO AllTheTypes(");

                    var separator = "";
                    foreach (var field in typeof (AllTheTypes).GetFields())
                    {
                        var sqlType = field.Name.Substring("The".Length).ToLower();

                        var typePostix = "";
                        if (field.Name.ToLower().Contains("var"))
                            typePostix = "(MAX)";

                        tableExpression.AppendLine(string.Format("    {0}[{1}] [{2}]{3} NOT NULL", separator, field.Name, sqlType, typePostix));
                        insertExpression.AppendLine(string.Format("     {0}{1}", separator, field.Name));
                        separator = ", ";
                    }

                    tableExpression.AppendLine(")");

                    separator = "";
                    insertExpression.AppendLine(") VALUES (");
                    foreach (var field in typeof (AllTheTypes).GetFields())
                    {
                        insertExpression.AppendLine(string.Format("     {0}@{1}", separator, field.Name));
                        separator = ", ";
                    }
                    insertExpression.AppendLine(")");

                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    foreach (var field in typeof(AllTheTypes).GetFields())
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
