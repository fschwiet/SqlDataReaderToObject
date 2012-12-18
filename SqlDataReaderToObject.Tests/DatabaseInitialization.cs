using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlDataReaderToObject.Tests
{
    public class DatabaseInitialization
    {
        static public void RecreateDatabase(string connectionString, string databaseMdfPath)
        {
            var databaseMdfFileInfo = new FileInfo(databaseMdfPath);
            if (!Directory.Exists(databaseMdfFileInfo.Directory.FullName))
                Directory.CreateDirectory(databaseMdfFileInfo.Directory.FullName);

            DetachDatabase(connectionString);

            var databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;

            ApplyAndRetryForDatabase("master", masterConnection =>
            {
                using (var createCommand = new SqlCommand(@"
USE Master
CREATE DATABASE $databaseName ON (NAME=DatabaseFile1, FILENAME= '$filename')
".Replace("$databaseName", databaseName).Replace("$filename", databaseMdfPath), masterConnection))
                {
                    createCommand.ExecuteNonQuery();
                }
            });
        }

        static public void DetachDatabase(string connectionString)
        {
            var c = new SqlConnectionStringBuilder(connectionString);

            var databaseToDrop = c.InitialCatalog;

            c.InitialCatalog = "master";

            ApplyAndRetryForDatabase(c.ToString(), connection =>
            {
                string script = @"
IF db_id('$databaseName') IS NOT NULL
BEGIN
    ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$databaseName]
END
".Replace("$databaseName", databaseToDrop);

                using (var command1 = new SqlCommand(script, connection))
                {
                    command1.ExecuteNonQuery();
                }
            });
        }

        public static void ApplyAndRetryForDatabase(string connectionString, Action<SqlConnection> task)
        {
            var retriesLeft = 3;

            while(retriesLeft-- > 0)
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        try
                        {
                            task(connection);
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                catch(SqlException e)
                {
                    // trying to be selective about what exceptions we retry on...
                    // In particular, retrying if the command failed due to a bad connection.
                    if (e.Message.Contains("transport-level"))
                        continue;

                    throw;
                }

                return;
            }
        }
    }
}