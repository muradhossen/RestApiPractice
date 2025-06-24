using Dapper;

namespace Movies.Application.Database
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public DbInitializer(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task InitializeAsync()
        {
            using var connection = await _connectionFactory.CreateAsync();
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS movies
                (
                    id UUID PRIMARY KEY,
                    title TEXT NOT NULL,
                    slug TEXT NOT NULL, 
                    yearofrelease int NOT NULL
                );
            ");

            await connection.ExecuteAsync("""
                CREATE unique index concurrently IF NOT EXISTS movies_slug_idx
                on movies
                using btree(slug);
            """);

            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS genres
                (
                    movieId UUID references movies(id),
                    name TEXT NOT NULL
                );
            ");

            await connection.ExecuteAsync("""
            create table if not exists ratings (
            userid uuid,
            movieid uuid references movies (id),
            rating integer not null,
            primary key (userid, movieid));
        """);

        }
    }
}
