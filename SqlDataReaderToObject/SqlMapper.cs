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
        public T ReadObject<T>(IDataReader reader) where T : new()
        {
            var type = typeof (T);
            Dictionary<string,Action<T,object>> setters = new Dictionary<string, Action<T, object>>();

            foreach (var field in type.GetFields().Where(f => f.IsPublic))
            {
                setters[field.Name] = (t, v) => field.SetValue(t, v);
            }

            T result = new T();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                Action<T, object> setter;

                if (setters.TryGetValue(name, out setter))
                {
                    setter(result, reader[i]);
                }
            }

            return result;
        }

        public IEnumerable<T> ReadObjects<T>(IDataReader reader) where T : new()
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
