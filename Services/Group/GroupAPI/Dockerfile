FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5032

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Group/GroupAPI/GroupAPI.csproj", "Services/Group/GroupAPI/"]
COPY ["Services/Group/GroupService.Application/GroupService.Application.csproj", "Services/Group/GroupService.Application/"]
COPY ["Services/Group/GroupService.Infrastructure/GroupService.Infrastructure.csproj", "Services/Group/GroupService.Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Services/Group/GroupService.Domain/GroupService.Domain.csproj", "Services/Group/GroupService.Domain/"]
RUN dotnet restore "./Services/Group/GroupAPI/GroupAPI.csproj"
COPY . .
WORKDIR "/src/Services/Group/GroupAPI"
RUN dotnet build "./GroupAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GroupAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GroupAPI.dll"]