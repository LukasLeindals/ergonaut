# syntax=docker/dockerfile:1.7

FROM ergonaut/proto-tools:latest AS proto-tools

ENV PROTOBUF_PROTOC=/usr/bin/protoc \
    Grpc_PluginFullPath=/usr/bin/grpc_csharp_plugin \
    GRPC_PROTOC_PLUGIN=/usr/bin/grpc_csharp_plugin

FROM proto-tools AS build
WORKDIR /src

# Seed restore cache with only project files so it survives code edits.
COPY Ergonaut.sln ./
COPY src/Ergonaut.Core/Ergonaut.Core.csproj src/Ergonaut.Core/
COPY src/Ergonaut.App/Ergonaut.App.csproj src/Ergonaut.App/
COPY src/Ergonaut.Infrastructure/Ergonaut.Infrastructure.csproj src/Ergonaut.Infrastructure/
COPY src/Ergonaut.UI/Ergonaut.UI.csproj src/Ergonaut.UI/
COPY src/Ergonaut.Sentinel/Ergonaut.Sentinel.csproj src/Ergonaut.Sentinel/
COPY src/Ergonaut.Api/Ergonaut.Api.csproj src/Ergonaut.Api/
COPY tests/Ergonaut.ArchTests/Ergonaut.ArchTests.csproj tests/Ergonaut.ArchTests/
COPY tests/Ergonaut.App.Tests/Ergonaut.App.Tests.csproj tests/Ergonaut.App.Tests/
COPY tests/Ergonaut.Api.Tests/Ergonaut.Api.Tests.csproj tests/Ergonaut.Api.Tests/

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore Ergonaut.sln

# Bring in the rest of the source after restore cache is primed.
COPY . .

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish src/Ergonaut.UI/Ergonaut.UI.csproj -c Release -o /out --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:5242 \
    DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Ergonaut.UI.dll"]
