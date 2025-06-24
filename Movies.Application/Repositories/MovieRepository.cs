using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private static readonly List<Movie> _movies = new();

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            this._dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> CreateMovieAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);
            using var transaction = connection.BeginTransaction();

            var result = await connection.ExecuteAsync(new CommandDefinition(@"
                INSERT INTO movies (id, title, slug, yearofrelease)
                VALUES (@Id, @Title, @Slug, @YearOfRelease);", movie, cancellationToken: token));

            if (result > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition(

                        @"
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name);", new
                        {
                            MovieId = movie.Id,
                            Name = genre
                        }, cancellationToken: token));
                }
            }

            transaction.Commit();
            return result > 0;
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);

            using var transaction = connection.BeginTransaction();

            await connection.ExecuteAsync(new CommandDefinition(@"
                DELETE FROM genres
                WHERE movieid = @Id;", new { Id = id }, cancellationToken: token));

            var result = await connection.ExecuteAsync(new CommandDefinition(@"
                DELETE FROM movies
                WHERE id = @Id;", new { Id = id }, cancellationToken: token));

            transaction.Commit();
            return result > 0;
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);
            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(@"
                SELECT EXISTS (
                    SELECT 1
                    FROM public.movies
                    WHERE id = @Id
                );", new { Id = id }, cancellationToken: token));
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);

            var orderClause = string.Empty;
            if (options.SortField is not null)
            {
                orderClause = $"""
            , m.{options.SortField}
            order by m.{options.SortField} {(options.SortOrder == SortOrder.Ascending ? "asc" : "desc")}
            """;
            }

            var result = await connection.QueryAsync(new CommandDefinition($"""
            select m.*, 
                   string_agg(distinct g.name, ',') as genres , 
                   round(avg(r.rating), 1) as rating, 
                   myr.rating as userrating
            from movies m 
            left join genres g on m.id = g.movieid
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid
                and myr.userid = @userId
            where (@title is null or m.title ilike ('%' || @title || '%'))
            and  (@yearofrelease is null or m.yearofrelease = @yearofrelease)
            group by id, userrating {orderClause}
            limit @pageSize
            offset @pageOffset
            """, new
            {
                userId = options.UserId,
                title = options.Title,
                yearofrelease = options.YearOfRelease,
                pageSize = options.PageSize,
                pageOffset = (options.Page - 1) * options.PageSize
            }, cancellationToken: token));

            return result.Select(x => new Movie
            {
                Id = x.id,
                Title = x.title,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(',')),
                Rating = (float?)x.rating,
                UserRating = x.userrating,

            });

        }

        public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);
            return await connection.QuerySingleAsync<int>(new CommandDefinition("""
            select count(id) from movies
            where (@title is null or title like ('%' || @title || '%'))
            and  (@yearOfRelease is null or yearofrelease = @yearOfRelease)
            """, new
            {
                title,
                yearOfRelease
            }, cancellationToken: token));
        }

        public async Task<Movie?> GetMovieByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
        new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating 
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid
                and myr.userid = @userId
            where id = @id
            group by id, userrating
            """, new { id, userId }, cancellationToken: token));

            if (movie is null)
            {
                return null;
            }

            var genras = await connection.QueryAsync<string>(new CommandDefinition(
               @"SELECT name FROM public.genres
                WHERE movieid = @Id;", new { Id = id }, cancellationToken: token));

            foreach (var genra in genras)
            {
                movie.Genres.Add(genra);
            }

            return movie;

        }

        public async Task<Movie?> GetMovieBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
            select m.*, round(avg(r.rating), 1) as rating, myr.rating as userrating
            from movies m
            left join ratings r on m.id = r.movieid
            left join ratings myr on m.id = myr.movieid
                and myr.userid = @userId
            where slug = @slug
            group by id, userrating
            """, new { slug, userId }, cancellationToken: token));

            if (movie is null)
            {
                return null;
            }

            var genras = await connection.QueryAsync<string>(new CommandDefinition(
               @"SELECT name FROM public.genres
                WHERE movieid = @Id;", new { Id = movie.Id }, cancellationToken: token));

            foreach (var genra in genras)
            {
                movie.Genres.Add(genra);
            }

            return movie;
        }

        public async Task<bool> UpdateMovieAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateAsync(token);
            using var transaction = connection.BeginTransaction();
            var result = await connection.ExecuteAsync(new CommandDefinition(@"
                UPDATE movies
                SET title = @Title, slug = @Slug, yearofrelease = @YearOfRelease
                WHERE id = @Id;", movie, cancellationToken: token));
            if (result > 0)
            {
                await connection.ExecuteAsync(new CommandDefinition(@"
                    DELETE FROM genres
                    WHERE movieid = @Id;", new { Id = movie.Id }, cancellationToken: token));
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition(@"
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name);", new
                    {
                        MovieId = movie.Id,
                        Name = genre
                    }, cancellationToken: token));
                }
            }
            transaction.Commit();
            return result > 0;

        }
    }
}
