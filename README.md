# Web Forum API

API for a Web Forum

## Build Status

[![.NET CI](https://github.com/JunaidZA/WebForum/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/JunaidZA/WebForum/actions/workflows/dotnet-ci.yml)

## Architecture

This project consists of:
- **Backend**: .NET 9 REST API

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- **[.NET SDK](https://dotnet.microsoft.com/download)** (9.0 or later)
- **[Git](https://git-scm.com/)**

### Installation
1. Clone the repo
```sh
git clone <your-repository-url>
cd WebForum
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

The API will be available at:
- **HTTP**: `http://localhost:5091`
- **HTTPS**: `https://localhost:7293`

### Configuration

- **Development**: `WebForum.Api/appsettings.Development.json`
- **Production**: `WebForum.Api/appsettings.Production.json`

## API Documentation

When running the API in development mode, Swagger documentation is available at:
`https://localhost:7293/swagger` or `http://localhost:5091/swagger`

OpenAPI json is available at:
`https://localhost:7293/openapi/v1.json` or `http://localhost:5091/openapi/v1.json`

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

**Tests failing**: Ensure all API dependencies are restored with `dotnet restore`.

