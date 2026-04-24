using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TMDBMovieSearch.Server
{
    public class WebServer
    {
        private readonly HttpListener _listener;
        private readonly string _prefix;

        public WebServer(string prefix)
        {
            _prefix = prefix;
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine($"Server slusa na {_prefix}");

            // petlja za osluskivanje
            while (true)
            {
                HttpListenerContext context = _listener.GetContext();
                // ovde ces proslediti zahtev na obradu
            }
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}
