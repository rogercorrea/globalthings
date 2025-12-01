FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./src/Monitor.Api ./src/Monitor.Api
RUN dotnet restore ./src/Monitor.Api/Monitor.Api.csproj
RUN dotnet publish ./src/Monitor.Api/Monitor.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Monitor.Api.dll"]
