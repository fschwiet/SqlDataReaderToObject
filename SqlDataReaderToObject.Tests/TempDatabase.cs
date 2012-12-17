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
        public string File { get; private set; }
        public string ConnectionString { get; private set; }
        private List<IDbConnection> _connections = new List<IDbConnection>(); 

        public static TempDatabase Create()
        {
            var file = Path.GetTempFileName();

            var connectionString = new SqlConnectionStringBuilder();
            connectionString.DataSource = file;
            connectionString.PersistSecurityInfo = false;

            
            SqlCeEngine en = new SqlCeEngine(connectionString.ToString());
            return new TempDatabase() { File = file, ConnectionString = en.LocalConnectionString };
        }

        private TempDatabase()
        {
        }

        public IDbConnection GetConnection()
        {
            var result = new SqlCeConnection(ConnectionString);
            _connections.Add(result);
            return result;
        }

        public void Dispose()
        {
            foreach (var connection in _connections)
            {
                connection.Close();
            }
            
            new FileInfo(File).Delete();
        }
    }
}
