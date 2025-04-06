# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5031


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Identity/IdentityAPI/IdentityAPI.csproj", "Services/Identity/IdentityAPI/"]
COPY ["Services/Identity/IdentityService.Application/IdentityService.Application.csproj", "Services/Identity/IdentityService.Application/"]
COPY ["Services/Identity/IdentityService.Domain/IdentityService.Domain.csproj", "Services/Identity/IdentityService.Domain/"]
COPY ["Services/Identity/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "Services/Identity/IdentityService.Infrastructure/"]
RUN dotnet restore "./Services/Identity/IdentityAPI/IdentityAPI.csproj"
COPY . .
WORKDIR "/src/Services/Identity/IdentityAPI"
RUN dotnet build "./IdentityAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityAPI.dll"]