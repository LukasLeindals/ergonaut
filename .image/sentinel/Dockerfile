# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN apt-get update \
 && apt-get install -y --no-install-recommends protobuf-compiler protobuf-compiler-grpc \
 && rm -rf /var/lib/apt/lists/*
ENV PROTOBUF_PROTOC=/usr/bin/protoc \
    Grpc_PluginFullPath=/usr/bin/grpc_csharp_plugin \
    GRPC_PROTOC_PLUGIN=/usr/bin/grpc_csharp_plugin

RUN dotnet restore src/Ergonaut.Sentinel/Ergonaut.Sentinel.csproj
RUN dotnet publish src/Ergonaut.Sentinel/Ergonaut.Sentinel.csproj \
    -c Release -o /out /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./
# keep config/data paths that Program.cs expects (../../config, ../../data)
COPY config /config
COPY data /data
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Ergonaut.Sentinel.dll"]
