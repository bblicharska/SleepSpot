# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5033


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Rental/RentalAPI/RentalAPI.csproj", "Services/Rental/RentalAPI/"]
COPY ["Services/Rental/RentalService.Application/RentalService.Application.csproj", "Services/Rental/RentalService.Application/"]
COPY ["Services/Rental/RentalService.Infrastructure/RentalService.Infrastructure.csproj", "Services/Rental/RentalService.Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Services/Rental/RentalService.Domain/RentalService.Domain.csproj", "Services/Rental/RentalService.Domain/"]
RUN dotnet restore "./Services/Rental/RentalAPI/RentalAPI.csproj"
COPY . .
WORKDIR "/src/Services/Rental/RentalAPI"
RUN dotnet build "./RentalAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RentalAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RentalAPI.dll"]