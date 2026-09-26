# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Orders.sln ./
COPY global.json Directory.Build.props ./
COPY src/Orders.Api/Orders.Api.csproj src/Orders.Api/
RUN dotnet restore src/Orders.Api/Orders.Api.csproj

COPY src/Orders.Api/ src/Orders.Api/
RUN dotnet publish src/Orders.Api/Orders.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER $APP_UID

COPY --from=build --chown=$APP_UID /app/publish .

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD curl -fsS http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "Orders.Api.dll"]
