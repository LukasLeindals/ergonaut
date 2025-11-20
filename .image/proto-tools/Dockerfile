# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:9.0

RUN --mount=type=cache,target=/var/cache/apt \
    --mount=type=cache,target=/var/lib/apt \
    apt-get update \
    && apt-get install -y --no-install-recommends protobuf-compiler protobuf-compiler-grpc \
    && rm -rf /var/lib/apt/lists/*

ENV PROTOBUF_PROTOC=/usr/bin/protoc \
    Grpc_PluginFullPath=/usr/bin/grpc_csharp_plugin \
    GRPC_PROTOC_PLUGIN=/usr/bin/grpc_csharp_plugin
