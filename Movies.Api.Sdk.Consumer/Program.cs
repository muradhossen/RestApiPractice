

using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Refit;
using System.Text.Json;

//var moviesApi = RestService.For<IMoviesApi>("https://localhost:7196");

//var movie =await moviesApi.GetMovieAsync("a69786f2-1f2d-40bc-8d84-d5b77bdfd35b");

var services = new ServiceCollection();   

services.AddRefitClient<IMoviesApi>()
    .ConfigureHttpClient(x =>
    {
        x.BaseAddress = new Uri("https://localhost:7196");
    });

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

var movies = await moviesApi.GetMoviesAsync(new Movies.Contracts.Requests.GetAllMoviesRequest { Page = 1, PageSize = 3 });

foreach (var item in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(item));
}

Console.ReadLine();
