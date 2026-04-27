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

                HttpListenerContext capturedContext = context; //captured-variable

                ThreadPool.QueueUserWorkItem(state =>
                {
                    HandleRequest(capturedContext);
                });
            }
        }

        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("Server je zaustavljen.");
        }

        private void HandleRequest(HttpListenerContext context)
        {
            Console.WriteLine($"[REQUEST] {context.Request.HttpMethod} {context.Request.Url}");

            string? query = context.Request.QueryString["query"]; //citamo query parametar
            
            if (string.IsNullOrEmpty(query))
            {
                SendResponse(context, 400, "Nedostaje query parametar. Primer: /search?query=Project+Hail+Mary");
                return;
            }

            //placeholder za sad -posle ide TMDBService poziv
            SendResponse(context, 200, $"Primljen zahtev za: {query}");
        }

        private void SendResponse(HttpListenerContext context, int statusCode, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message); //HTTP salje bytes

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);

            context.Response.OutputStream.Close();
        }
    }
}
