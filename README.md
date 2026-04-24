# TMDB Movie Search Server

Concurrent C# web server for searching movies via TMDB API with thread-safe caching and thread synchronization.

## Project Description

A server application that:
- Enables clients to search for movies through a web browser (GET requests)
- Uses TMDB API to fetch movie data
- Implements thread-safe caching to reduce API calls
- Processes multiple requests concurrently using ThreadPool
- Maintains thread-safe logging and error handling

## Example Request

http://localhost:8080/search?query=Avatar


## Requirements

- .NET 6.0 or higher
- TMDB API key (free at https://www.themoviedb.org/settings/api)

## Installation

### 1. Clone the repository
```bash
git clone https://github.com/Nero265Hej/tmdb-movie-search-server.git
cd tmdb-movie-search-server