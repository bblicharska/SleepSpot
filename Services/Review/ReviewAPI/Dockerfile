FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5029

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

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReviewAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewAPI.dll"]