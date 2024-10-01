FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SMAIAXBackend.API/SMAIAXBackend.API.csproj", "SMAIAXBackend.API/"]
COPY ["src/SMAIAXBackend.Application/SMAIAXBackend.Application.csproj", "SMAIAXBackend.Application/"]
COPY ["src/SMAIAXBackend.Domain/SMAIAXBackend.Domain.csproj", "SMAIAXBackend.Domain/"]
COPY ["src/SMAIAXBackend.Infrastructure/SMAIAXBackend.Infrastructure.csproj", "SMAIAXBackend.Infrastructure/"]
RUN dotnet restore "SMAIAXBackend.API/SMAIAXBackend.API.csproj"
COPY src/ .
WORKDIR "SMAIAXBackend.API"
RUN dotnet build "SMAIAXBackend.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SMAIAXBackend.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SMAIAXBackend.API.dll"]
