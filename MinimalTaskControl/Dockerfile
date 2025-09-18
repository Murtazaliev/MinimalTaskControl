FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MinimalTaskControl.sln", "."]

COPY ["MinimalTaskControl.Core/*.csproj", "MinimalTaskControl.Core/"]
COPY ["MinimalTaskControl.Infrastructure/*.csproj", "MinimalTaskControl.Infrastructure/"]
COPY ["MinimalTaskControl/*.csproj", "MinimalTaskControl/"]
COPY ["MinimalTaskControl.Core.Tests/*.csproj", "MinimalTaskControl.Core.Tests/"]
COPY ["MinimalTaskControl.Infrastructure.Tests/*.csproj", "MinimalTaskControl.Infrastructure.Tests/"]

RUN dotnet restore "MinimalTaskControl.sln" --ignore-failed-sources

COPY . .

WORKDIR "/src/MinimalTaskControl"
RUN dotnet build "MinimalTaskControl.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MinimalTaskControl.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalTaskControl.WebApi.dll"]