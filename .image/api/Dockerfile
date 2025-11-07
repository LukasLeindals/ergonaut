FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN apt-get update \
 && apt-get install -y --no-install-recommends protobuf-compiler protobuf-compiler-grpc \
 && rm -rf /var/lib/apt/lists/*
ENV PROTOBUF_PROTOC=/usr/bin/protoc \
    Grpc_PluginFullPath=/usr/bin/grpc_csharp_plugin \
    GRPC_PROTOC_PLUGIN=/usr/bin/grpc_csharp_plugin

RUN dotnet restore src/Ergonaut.Api/Ergonaut.Api.csproj
RUN dotnet publish src/Ergonaut.Api/Ergonaut.Api.csproj -c Release -o /out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:5075 \
    DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Ergonaut.Api.dll"]
