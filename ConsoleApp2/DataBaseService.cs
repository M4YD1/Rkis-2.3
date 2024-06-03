using Npgsql;

public class DataBaseService
{
    private static NpgsqlConnection? _connection;
    
    private static string GetConnectionString()
    {
        return @"Host=10.30.0.137;Port=5432;Database=gr624_eshrso;Username=gr624_eshrso;Password=ishkobilka";
    }
    
    public static NpgsqlConnection GetSqlConnection()
    {
        if (_connection is null)
        {
            _connection = new NpgsqlConnection(GetConnectionString());
            _connection.Open();
        }
        
        return _connection;
    }
}