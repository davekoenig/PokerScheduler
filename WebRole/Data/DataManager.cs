using System;

namespace WebRole.Data
{
    public class DataManager
    {
        private static Lazy<DataManager> _instance = new Lazy<DataManager>(() => new DataManager());

        public static DataManager Instance => _instance.Value;

        private DataManager()
        {
            Provider = new TableStorageDataProvider();
        }

        public IDataProvider Provider { get; private set; }
    }
}