FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
RUN apt-get update \
    && apt-get upgrade -y \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN ls -la
RUN dotnet restore
RUN ls -la
WORKDIR /src/TestConsoleApp
RUN dotnet add package Emgu.CV.runtime.ubuntu.20.04-x64
RUN dotnet build -c Release -o /app/build
RUN ls -la
RUN dotnet publish -c Release -o /app/publish
RUN ls -la

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish ./
RUN ls -la
ENTRYPOINT ["dotnet", "TestConsoleApp.dll"]