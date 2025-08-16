FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5031

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Identity/IdentityAPI/IdentityAPI.csproj", "Services/Identity/IdentityAPI/"]
COPY ["Services/Identity/IdentityService.Application/IdentityService.Application.csproj", "Services/Identity/IdentityService.Application/"]
COPY ["Services/Identity/IdentityService.Domain/IdentityService.Domain.csproj", "Services/Identity/IdentityService.Domain/"]
COPY ["Services/Identity/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "Services/Identity/IdentityService.Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "./Services/Identity/IdentityAPI/IdentityAPI.csproj"
COPY . .
WORKDIR "/src/Services/Identity/IdentityAPI"
RUN dotnet build "./IdentityAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityAPI.dll"]






