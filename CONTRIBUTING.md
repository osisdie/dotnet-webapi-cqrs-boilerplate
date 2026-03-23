# Contributing to dotnet-mediatr-boilerplate

Thank you for your interest in contributing! Here's how to get started.

## Development Setup

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (optional, for container builds)

### Build & Run

```bash
# Clone the repository
git clone https://github.com/osisdie/dotnet-mediatr-boilerplate.git
cd dotnet-mediatr-boilerplate

# Restore and build
dotnet restore hello-mediatR-all-projects.sln
dotnet build hello-mediatR-all-projects.sln -c Release

# Run locally
dotnet run --project src/Endpoint/HelloMediatR/Hello.MediatR.Endpoint.csproj
# Visit http://localhost:5000/swagger
```

### Run Tests

```bash
export ASPNETCORE_ENVIRONMENT=Debug
export COREFX_API_NAME=hello-mediatr-api-debug
dotnet test hello-mediatR-all-projects.sln -c Release
```

## How to Contribute

1. **Fork** the repository
2. **Create a branch** (`git checkout -b feature/your-feature`)
3. **Make your changes** following the code style (`.editorconfig` is included)
4. **Run tests** to ensure everything passes
5. **Commit** with a clear message (e.g., `feat: add PostgreSQL support`)
6. **Push** and open a **Pull Request**

## Commit Messages

We follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` new feature
- `fix:` bug fix
- `refactor:` code refactoring
- `docs:` documentation changes
- `test:` adding or updating tests
- `chore:` maintenance tasks

## Code Style

This project uses `.editorconfig` for consistent formatting. Please ensure your editor respects it.

## Questions?

Open an [issue](https://github.com/osisdie/dotnet-mediatr-boilerplate/issues) and we'll be happy to help.
