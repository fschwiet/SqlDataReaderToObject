using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDataReaderToObject.Tests
{
    public class TempDatabase : IDisposable
    {
        public string Name { get; private set; }
        public string ConnectionString { get; private set; }
        private List<IDbConnection> _connections = new List<IDbConnection>(); 

        public static TempDatabase Create()
        {
            var databaseName = typeof (TempDatabase).Namespace.Replace(".", "_");

            var databaseFilepath = Path.Combine("c:\\temp", "Sql.mdf");

            var connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = ".\\sqlexpress";
            connectionString.InitialCatalog = databaseName;
            connectionString.IntegratedSecurity = true;

            DatabaseInitialization.DetachDatabase(connectionString.ToString());

            if (File.Exists(databaseFilepath))
                File.Delete(databaseFilepath);
            
            connectionString.InitialCatalog = "master";
            
            using (var masterDatabaseConnection = new SqlConnection(connectionString.ToString()))
            {
                masterDatabaseConnection.Open();

                var createSqlText = string.Format(@"
                        CREATE DATABASE [{0}] ON PRIMARY (NAME={0}, FILENAME='{1}')", databaseName, databaseFilepath);

                using (var createCommand = new SqlCommand(createSqlText, masterDatabaseConnection))
                {
                    createCommand.ExecuteNonQuery();
                }
            }

            connectionString.InitialCatalog = databaseName;

            return new TempDatabase() { Name = databaseName, ConnectionString = connectionString.ToString()};
        }

        private TempDatabase()
        {
        }

        public IDbConnection GetConnection()
        {
            var result = new SqlConnection(ConnectionString);
            _connections.Add(result);
            return result;
        }

        public void Dispose()
        {
            foreach (var connection in _connections)
            {
                connection.Close();
            }
            
            new FileInfo(Name).Delete();
        }

        public void RunNonQuery(string createTableFooColumn1BigintNotNull, Dictionary<string,object> parameters = null)
        {
            parameters = parameters ?? new Dictionary<string, object>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (IDbCommand command = connection.CreateCommand())
                {
                    foreach (var parameter in parameters)
                    {
                        var dbParameter = command.CreateParameter();
                        dbParameter.ParameterName = parameter.Key;
                        dbParameter.Value = parameter.Value;
                        command.Parameters.Add(dbParameter);
                    }


                    command.CommandText = createTableFooColumn1BigintNotNull;
                    command.ExecuteNonQuery();
                }
            }
        }

        public object RunScalar(string selectCountFromFoo)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = selectCountFromFoo;
                    return command.ExecuteScalar();
                }
            }
        }

        public void RunQuery(Action<IDataReader> handler)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Foo";

                    using (var reader = command.ExecuteReader())
                    {
                        handler(reader);
                    }
                }
            }
        }
    }
}
