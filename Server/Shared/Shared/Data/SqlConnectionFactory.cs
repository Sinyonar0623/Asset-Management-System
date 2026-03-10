using System.Data;
using Npgsql;

namespace Shared.Data;

public class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory, IDisposable
{
    private readonly string _connectionString = connectionString;
    private IDbConnection? _connection;

    public IDbConnection CreateNewConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        return connection;
    }

    public void Dispose()
    {
        if (_connection is not null && _connection.State == ConnectionState.Open)
        {
            _connection.Dispose();
        }
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }

    public IDbConnection GetOpenConnection()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
        }
        return _connection;
    }
}
