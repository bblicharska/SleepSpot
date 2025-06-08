# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5032


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Group/GroupAPI/GroupAPI.csproj", "Services/Group/GroupAPI/"]
COPY ["Services/Group/GroupService.Application/GroupService.Application.csproj", "Services/Group/GroupService.Application/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Services/Group/GroupService.Domain/GroupService.Domain.csproj", "Services/Group/GroupService.Domain/"]
RUN dotnet restore "./Services/Group/GroupAPI/GroupAPI.csproj"
COPY . .
WORKDIR "/src/Services/Group/GroupAPI"
RUN dotnet build "./GroupAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GroupAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroupAPI.dll"]