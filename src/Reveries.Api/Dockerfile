FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Reveries.Api/Reveries.Api.csproj", "src/Reveries.Api/"]
COPY ["src/Reveries.Application/Reveries.Application.csproj", "src/Reveries.Application/"]
COPY ["src/Reveries.Core/Reveries.Core.csproj", "src/Reveries.Core/"]
COPY ["src/Reveries.Contracts/Reveries.Contracts.csproj", "src/Reveries.Contracts/"]
COPY ["src/Reveries.Infrastructure.Postgresql/Reveries.Infrastructure.Postgresql.csproj", "src/Reveries.Infrastructure.Postgresql/"]
COPY ["src/Reveries.Infrastructure.Redis/Reveries.Infrastructure.Redis.csproj", "src/Reveries.Infrastructure.Redis/"]
COPY ["src/Reveries.Integration.GoogleBooks/Reveries.Integration.GoogleBooks.csproj", "src/Reveries.Integration.GoogleBooks/"]
COPY ["src/Reveries.Integration.Isbndb/Reveries.Integration.Isbndb.csproj", "src/Reveries.Integration.Isbndb/"]

RUN dotnet restore "src/Reveries.Api/Reveries.Api.csproj"

COPY src/ src/

WORKDIR "/src/Reveries.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Reveries.Api.dll"]