# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5030


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Services/Property/PropertyAPI/PropertyAPI.csproj", "Services/Property/PropertyAPI/"]
COPY ["Services/Property/Property.Application/PropertyService.Application.csproj", "Services/Property/Property.Application/"]
COPY ["Services/Property/Property.Domain/PropertyService.Domain.csproj", "Services/Property/Property.Domain/"]
COPY ["Services/Property/Property.Infrastructure/PropertyService.Infrastructure.csproj", "Services/Property/Property.Infrastructure/"]
RUN dotnet restore "./Services/Property/PropertyAPI/PropertyAPI.csproj"
COPY . .
WORKDIR "/src/Services/Property/PropertyAPI"
RUN dotnet build "./PropertyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PropertyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropertyAPI.dll"]