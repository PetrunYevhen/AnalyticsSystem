FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["src/Analytics/Analytics.API/Analytics.API.csproj", "src/Analytics/Analytics.API/"]
COPY ["src/Analytics/Analytics.Application/Analytics.Application.csproj", "src/Analytics/Analytics.Application/"]
COPY ["src/Analytics/Analytics.Domain/Analytics.Domain.csproj", "src/Analytics/Analytics.Domain/"]
COPY ["src/Analytics/Analytics.Infrastructure/Analytics.Infrastructure.csproj", "src/Analytics/Analytics.Infrastructure/"]
COPY ["src/BuildingBlocks/Application/Application.csproj", "src/BuildingBlocks/Application/"]
COPY ["src/BuildingBlocks/Domain/Domain.csproj", "src/BuildingBlocks/Domain/"]
COPY ["src/BuildingBlocks/Infrastructure/Infrastructure.csproj", "src/BuildingBlocks/Infrastructure/"]
COPY ["src/SharedKernel/SharedKernel.csproj", "src/SharedKernel/"]
COPY ["Directory.Packages.props", "./"]


RUN dotnet restore "src/Analytics/Analytics.API/Analytics.API.csproj"

COPY . .
RUN dotnet publish "src/Analytics/Analytics.API/Analytics.API.csproj" \
    -c Release -o /out --no-restore 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Analytics.API.dll"]


