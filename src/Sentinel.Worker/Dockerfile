FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sentinel.Domain/Sentinel.Domain.csproj", "Sentinel.Domain/"]
COPY ["Sentinel.Application/Sentinel.Application.csproj", "Sentinel.Application/"]
COPY ["Sentinel.Infrastructure/Sentinel.Infrastructure.csproj", "Sentinel.Infrastructure/"]
COPY ["Sentinel.Worker/Sentinel.Worker.csproj", "Sentinel.Worker/"]
RUN dotnet restore "Sentinel.Worker/Sentinel.Worker.csproj"
COPY . .
WORKDIR "/src/Sentinel.Worker"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sentinel.Worker.dll"]
