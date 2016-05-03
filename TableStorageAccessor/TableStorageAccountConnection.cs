namespace Microsoft.SportsCloud.TableStorage
{
    /// <summary>
    /// Wrapper class around a TableStorage Account
    /// (only a connection string for now: mostly for extensibility)
    /// </summary>
    public class TableStorageAccountConnection
    {
        /// <summary>
        /// Connection String for the Table Storage Account
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">connection string</param>
        public TableStorageAccountConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
