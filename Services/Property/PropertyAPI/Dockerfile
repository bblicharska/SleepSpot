FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5030

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

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PropertyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

COPY --from=build /src/Services/Property/PropertyAPI/wwwroot ./wwwroot/

RUN chmod -R 755 /app/wwwroot

VOLUME ["/app/wwwroot/uploads"]

ENTRYPOINT ["dotnet", "PropertyAPI.dll"]