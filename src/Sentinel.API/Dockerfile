FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5052
EXPOSE 5053

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sentinel.Domain/Sentinel.Domain.csproj", "Sentinel.Domain/"]
COPY ["Sentinel.Application/Sentinel.Application.csproj", "Sentinel.Application/"]
COPY ["Sentinel.Infrastructure/Sentinel.Infrastructure.csproj", "Sentinel.Infrastructure/"]
COPY ["Sentinel.API/Sentinel.API.csproj", "Sentinel.API/"]
RUN dotnet restore "Sentinel.API/Sentinel.API.csproj"
COPY . .
WORKDIR "/src/Sentinel.API"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sentinel.API.dll"]
