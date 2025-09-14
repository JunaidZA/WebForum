# Web Forum API

API for a Web Forum

## Build Status

[![.NET CI](https://github.com/JunaidZA/WebForum/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/JunaidZA/WebForum/actions/workflows/dotnet-ci.yml)

## Architecture

This project consists of:
- **Backend**: .NET 9 REST API
- **Database**: Local Sqlite

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- **[.NET SDK](https://dotnet.microsoft.com/download)** (9.0 or later)
- **[Git](https://git-scm.com/)**

### Installation
1. Clone the repo
```sh
git clone https://github.com/JunaidZA/WebForum
```

2. Restore dependencies
```sh
dotnet restore
```

3. Build the solution
```sh
dotnet build
```

4. Run the API
```sh
cd WebForum.Api
dotnet run
```

The API will be available at one of the following:
- **HTTP**: `http://localhost:5091`
- **HTTPS**: `https://localhost:7293`

### Configuration

- **Development**: `WebForum.Api/appsettings.Development.json`
- **Production**: `WebForum.Api/appsettings.Production.json`

## Database

The API uses a local SQLite database with Entity Framework. 
To create a fresh new DB delete all files in `WebForum.Api/Data`. The API will create a new DB on startup.

If you make a change to the DB structure and want to create a migration:

Make sure Dotnet-ef is installed globally:
```sh
dotnet tool install --global dotnet-ef
```

Create a migration:
```sh
dotnet ef migrations add <migration name> --project WebForum.Infrastructure --startup-project WebForum.Api
```

The DB will be migrated on API startup.

## API Documentation

When running the API in development mode, Swagger documentation is available at:
`https://localhost:7293/swagger` or `http://localhost:5091/swagger`

OpenAPI json is available at:
`https://localhost:7293/openapi/v1.json` or `http://localhost:5091/openapi/v1.json`

## Testing the API locally using Postman/Bruno

I prefer to use [Bruno](https://www.usebruno.com/downloads) for my API calls.

The Bruno collections are in `WebForum.Api/Bruno Collections`.

Alternatively there is an export of the collection in Postman format in `WebForum.Api/Bruno Collections` which can be imported into Postman.

### Collection Usage for local testing

There are 2 users created in the existing DB committed to source control, the login details are the two Login requests stored in the Users folder in the collection.

The requests in the Posts folder inherit their authentication from the folders authentication, to generate a JWT call the Login user endpoint with your users email and password then use the returned JWT as the Bearer auth for other requests.

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Run tests to ensure quality
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Submit a pull request

## Troubleshooting

### Common Issues

**API not starting**: Ensure .NET 9 SDK is installed and ports 5091/7293 are available.
