# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5030

# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Services/Property/PropertyAPI/PropertyAPI.csproj", "Services/Property/PropertyAPI/"]
COPY ["Services/Property/Property.Application/PropertyService.Application.csproj", "Services/Property/Property.Application/"]
COPY ["Services/Property/Property.Domain/PropertyService.Domain.csproj", "Services/Property/Property.Domain/"]
COPY ["Services/Property/Property.Infrastructure/PropertyService.Infrastructure.csproj", "Services/Property/Property.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "./Services/Property/PropertyAPI/PropertyAPI.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/Services/Property/PropertyAPI"

# Build the application
RUN dotnet build "./PropertyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PropertyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# WAŻNE: Skopiuj istniejący folder wwwroot z twoimi zdjęciami
COPY --from=build /src/Services/Property/PropertyAPI/wwwroot ./wwwroot/

# Ustaw odpowiednie uprawnienia
RUN chmod -R 755 /app/wwwroot

# Opcjonalnie: Volume dla przyszłych uploadów
VOLUME ["/app/wwwroot/uploads"]

ENTRYPOINT ["dotnet", "PropertyAPI.dll"]