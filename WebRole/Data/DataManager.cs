using System;

namespace WebRole.Data
{
    public class DataManager
    {
        private static Lazy<DataManager> _instance = new Lazy<DataManager>(() => new DataManager());

        public static DataManager Instance => _instance.Value;

        private DataManager()
        {

        }

        public static IDataProvider Provider { get; }
    }
}