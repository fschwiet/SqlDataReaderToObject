using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlDataReaderToObject
{
    public class SqlMapper
    {
        public T ReadObject<T>(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> ReadObjects<T>(IDataReader reader)
        {
            List<T> results = new List<T>();
            while (reader.Read())
            {
                results.Add(ReadObject<T>(reader));
            }

            return results;
        }
    }
}
