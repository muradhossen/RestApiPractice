using Npgsql;
using System.Data;

namespace Movies.Application.Database
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateAsync(CancellationToken token = default);
    }
    public class NpgSqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string connectionString;

        public NpgSqlConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<IDbConnection> CreateAsync(CancellationToken token = default)
        {
            NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync(token);
            return connection;
        }
    }
}
