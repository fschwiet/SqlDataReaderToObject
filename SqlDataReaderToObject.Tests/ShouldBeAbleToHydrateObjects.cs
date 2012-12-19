﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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
            var database = arrange(() => TempDatabase.Create());

            given("a basic table with a row", () =>
            {
                arrange(() =>
                {
                    database.RunNonQuery("CREATE TABLE Foo([column1] [bigint] NOT NULL)");
                    database.RunNonQuery("INSERT INTO Foo (column1) Values (123)");
                });

                then("we should be able to read a row", () =>
                {
                    Foo result = null;

                    database.RunQuery("SELECT * FROM Foo", r => { result = new SqlMapper().ReadObjects<Foo>(r).Single(); });

                    expect(() => result.column1 == 123);
                });
            });

            given("a table allowing nulls", () =>
            {
                arrange(() =>
                {
                    CreateTableForType(database, "AllTheTypes", typeof (AllTheTypes));
                });

                when("we insert an object without nullables", () =>
                {
                    var dto = new AllTheTypes();

                    arrange(() =>
                    {
                        InsertObject(database, "AllTheTypes", dto);
                    });

                    then("we should be able to read the row", () =>
                    {
                        AllTheTypes result = null;

                        database.RunQuery("SELECT * FROM AllTheTypes", r => { result = new SqlMapper().ReadObjects<AllTheTypes>(r).Single(); });

                        result.TheTimeStamp = null;  // timestamp is generated by the database

                        var serializedResult = JsonConvert.SerializeObject(result);
                        var expectedResult = JsonConvert.SerializeObject(dto);
                        expect(() => serializedResult == expectedResult);
                    });

                    then("we should be able to read the row into a type with nullables", () =>
                    {
                        AllTheNullableTypes result = null;

                        database.RunQuery("SELECT * FROM AllTheTypes", r => { result = new SqlMapper().ReadObjects<AllTheNullableTypes>(r).Single(); });

                        result.TheTimeStamp = null;  // timestamp is generated by database

                        var serializedResult = JsonConvert.SerializeObject(result);
                        var expectedResult = JsonConvert.SerializeObject(new AllTheTypes());
                        expect(() => serializedResult == expectedResult);
                    });
                });

                when("we insert an object with nullables", () =>
                {
                    var dto = new AllTheNullableTypes();

                    arrange(() => InsertObject(database, "AllTheTypes", dto));

                    then("we should be able to read the row", () =>
                    {
                        AllTheNullableTypes result = null;

                        database.RunQuery("SELECT * FROM AllTheTypes", r => { result = new SqlMapper().ReadObjects<AllTheNullableTypes>(r).Single(); });

                        result.TheTimeStamp = null;  // timestamp is generated by the database

                        var serializedResult = JsonConvert.SerializeObject(result);
                        var expectedResult = JsonConvert.SerializeObject(dto);
                        expect(() => serializedResult == expectedResult);
                    });
                });
            });
        }

        private static void InsertObject(TempDatabase database, string tableName, object dto)
        {
            var insertedFields = dto.GetType().GetFields().Where(f => f.Name != "TheTimeStamp");
            var insertExpression = new StringBuilder();
            insertExpression.AppendLine("INSERT INTO " + tableName + "(");

            var separator = "";
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

            database.RunNonQuery(insertExpression.ToString(), parameters);
        }

        private static void CreateTableForType(TempDatabase database, string tableName, Type type)
        {
            var fields = type.GetFields();

            var tableExpression = new StringBuilder();
            tableExpression.AppendLine("CREATE TABLE " + tableName + "(");

            var separator = "";
            foreach (var field in fields)
            {
                var sqlType = field.Name.Substring("The".Length).ToLower();

                tableExpression.AppendLine(string.Format("    {0}[{1}] [{2}]", separator, field.Name, sqlType));
                separator = ", ";
            }

            tableExpression.AppendLine(")");

            database.RunNonQuery(tableExpression.ToString());
        }
    }
}
