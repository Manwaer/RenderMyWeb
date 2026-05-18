FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Исправлено: .csproj лежит рядом с Dockerfile, без подпапки
COPY ["WorkoutLog.csproj", "."]
RUN dotnet restore "./WorkoutLog.csproj"

COPY . .

RUN dotnet build "./WorkoutLog.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WorkoutLog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render.com передаёт порт через переменную PORT
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080

ENTRYPOINT ["dotnet", "WorkoutLog.dll"]