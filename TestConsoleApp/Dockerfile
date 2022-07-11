FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestConsoleApp/TestConsoleApp.csproj", "TestConsoleApp/"]
COPY ["OCRlmplementaion/OCRlmplementaion.csproj", "OCRlmplementaion/"]
RUN dotnet restore
RUN dotnet add package Emgu.CV.runtime.ubuntu.20.04-x64
COPY . .
WORKDIR "/src/TestConsoleApp"
RUN dotnet build -c Debug -r ubuntu.20.04-x64 -o /app/build

FROM build AS publish
RUN dotnet publish -c Debug -r ubuntu.20.04-x64 -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mv bin/Debug/net6.0/ /app
ENTRYPOINT ["dotnet", "TestConsoleApp.dll"]