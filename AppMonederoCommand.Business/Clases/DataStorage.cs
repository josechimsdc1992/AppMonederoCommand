using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppMonederoCommand.Business.Clases
{
    public class DataStorage
    {
        private List<KeyValuePair<string, object>> persistentData = new List<KeyValuePair<string, object>>();

        public DataStorage()
        {
        }

        public void AddData(string key, object data)
        {
            persistentData.Add(new KeyValuePair<string, object>(key, data));
        }

        public List<KeyValuePair<string, object>> GetAllData()
        {
            return persistentData;
        }

        public object GetAllData(string key)
        {
            return persistentData.FirstOrDefault(e => e.Key == key).Value;
        }

        public T? Get<T>(string key)
        {
            object value = persistentData.LastOrDefault(e => e.Key == key).Value;

            if (value is T castedValue)
            {
                return castedValue;
            }
            else
            {
                return default;
            }

        }

    }
}
