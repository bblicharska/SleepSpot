# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5029


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Services/Review/ReviewAPI/ReviewAPI.csproj", "Services/Review/ReviewAPI/"]
COPY ["Services/Review/ReviewService.Application/ReviewService.Application.csproj", "Services/Review/ReviewService.Application/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Services/Review/ReviewService.Domain/ReviewService.Domain.csproj", "Services/Review/ReviewService.Domain/"]
COPY ["Services/Review/ReviewService.Infrastructure/ReviewService.Infrastructure.csproj", "Services/Review/ReviewService.Infrastructure/"]
RUN dotnet restore "./Services/Review/ReviewAPI/ReviewAPI.csproj"
COPY . .
WORKDIR "/src/Services/Review/ReviewAPI"
RUN dotnet build "./ReviewAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReviewAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewAPI.dll"]