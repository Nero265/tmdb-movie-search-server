using TMDBMovieSearch.Server;
using DotNetEnv;


class Program
{
    static void Main(string[] args)
    {
        Env.Load();

        var server = new WebServer("http://localhost:5000/");
        server.Start();
    }
}
