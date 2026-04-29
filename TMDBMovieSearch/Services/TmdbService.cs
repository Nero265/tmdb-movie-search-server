using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDBMovieSearch.Cache;

namespace TMDBMovieSearch.Services
{
    public class TmdbService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly HttpClient _client;

        private readonly Dictionary<string, CacheEntry> _cache = new();

        private readonly object _cacheLock = new object();
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(5);

        public TmdbService(string baseUrl, string apiKey, HttpClient client)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
            _client = client;
        }

        public JObject Search(string query)
        {
            string cacheKey = GenerateCacheKey(query);
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (_cache.TryGetValue(cacheKey, out CacheEntry? entry ))
            {
                if (entry.IsExpired)
                {
                    _cache.Remove(cacheKey);
                    Console.WriteLine($"[CACHE EXPIRED] '{query}' -> uklonjen iz kesa");
                }
                else
                {
                    stopwatch.Stop();
                    Console.WriteLine($"[CACHE HIT] '{query}' -> {stopwatch.ElapsedMilliseconds}ms ");
                    return entry.Value;
                }
                   
            }

            //nije u kesu
            lock(_cacheLock)
            {
                if (_cache.TryGetValue(cacheKey, out CacheEntry? entryInner))
                {
                    if (entryInner.IsExpired)
                    {
                        _cache.Remove(cacheKey);
                        Console.WriteLine($"[CACHE EXPIRED] '{query}' -> uklonjen iz kesa");
                    }

                    else
                    {
                        stopwatch.Stop();
                        Console.WriteLine($"[CACHE HIT] '{query}' -> {stopwatch.ElapsedMilliseconds}ms ");
                        return entryInner.Value;
                    }
                }

                //sigurno nema
                Console.WriteLine($"[CACHE MISS] '{query} -> pozivam TMDB API...");

                try
                {
                    JObject result = CallApi(query);
                    _cache[cacheKey] = new CacheEntry(result, _cacheTtl);

                    stopwatch.Stop();
                    Console.WriteLine($"[CACHED]\t\t'{query}' -> {stopwatch.ElapsedMilliseconds}ms");
                    return result;
                }
                catch(Exception e)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"[ERROR]\t\t'{query}' -> {e.Message}");
                    throw;
                }
            }
        }

        private string GenerateCacheKey(string query)
        {
            var allParams = new Dictionary<string, string>
            {
                { "query", query.Trim().ToLowerInvariant() }
            };


            //redosled parametra -radi
            var sorted = allParams.OrderBy(p => p.Key);
            return string.Join("&", sorted.Select(p => $"{p.Key}={p.Value}"));
        }

        private JObject CallApi(string query)
        {
            string url = $"{_baseUrl}?query={Uri.EscapeDataString(query)}&api_key={_apiKey}";

            HttpResponseMessage response = _client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            string body = response.Content.ReadAsStringAsync().Result;
            return JObject.Parse(body);
        }

        public void PrintCacheStats()
        {
            lock(_cacheLock)
            {
                Console.WriteLine("\n======== Cache stanje ========");
                Console.WriteLine($"\t Unosa u kesu: {_cache.Count}");
                Console.WriteLine($"\t TTL: {_cacheTtl.TotalMinutes} minuta");
                Console.WriteLine("\t Unosi:");

                foreach (var p in _cache)
                {
                    string status = p.Value.IsExpired ?
                        "ISTEKAO" : $"istice za {(p.Value.ExpiresAt - DateTime.UtcNow).TotalSeconds:F0}s";

                    Console.WriteLine($"\t\t [{status}] '{p.Key}'");
                }

                Console.WriteLine("==============================\n");
            }
        }

    }
}
