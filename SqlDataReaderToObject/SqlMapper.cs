using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlDataReaderToObject
{
    public class SqlMapper
    {
        public T ReadObject<T>(IDataReader reader) where T : new()
        {
            return (T)ReadObject(reader, typeof (T));
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

        public object ReadObject(IDataReader reader, Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);
            Dictionary<string, Action<object, object>> setters = new Dictionary<string, Action<object, object>>();

            foreach (var field in type.GetFields().Where(f => f.IsPublic))
            {
                setters[field.Name] = (t, v) => field.SetValue(t, v);
            }

            var result = constructor.Invoke(new object[0]);

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                Action<object, object> setter;

                if (setters.TryGetValue(name, out setter))
                {
                    var value = reader[i];

                    if (value == DBNull.Value)
                        value = null;

                    setter(result, value);
                }
            }

            return result;
        }

        public IEnumerable<object> ReadObjects(IDataReader reader, Type type)
        {
            List<object> results = new List<object>();
            while (reader.Read())
            {
                results.Add(ReadObject(reader, type));
            }

            return results;
        }
    }
}
