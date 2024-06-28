using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Order.Outbox.Inbox.Table.Publisher.Service
{
    public static class OrderSingletonDatabase
    {
        static IDbConnection _connection;
        static bool _dataReaderState = true;
        static OrderSingletonDatabase()
            => _connection = new SqlConnection("Server=localhost;Database=OutboxInboxDb;Trusted_Connection=True;TrustServerCertificate=True;");
        public static IDbConnection Connection
        {
            get
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                return _connection;
            }
        }
        public static async Task<IEnumerable<T>> QueryAsync<T>(string sql)
             => await _connection.QueryAsync<T>(sql);
        public static async Task<int> ExecuteAsync(string sql)
            => await _connection.ExecuteAsync(sql);
        public static void DataReaderReady()
            => _dataReaderState = true;
        public static void DataReaderBusy()
            => _dataReaderState = false;
        public static bool DataReaderState => _dataReaderState;
    }
}
