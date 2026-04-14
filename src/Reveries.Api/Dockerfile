FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Reveries.Api/Reveries.Api.csproj", "Reveries.Api/"]
COPY ["src/Reveries.Application/Reveries.Application.csproj", "Reveries.Application/"]
COPY ["src/Reveries.Domain/Reveries.Domain.csproj", "Reveries.Domain/"]
COPY ["src/Reveries.Contracts/Reveries.Contracts.csproj", "Reveries.Contracts/"]
COPY ["src/Reveries.Infrastructure/Reveries.Infrastructure.csproj", "Reveries.Infrastructure/"]
COPY ["src/Reveries.Infrastructure.Postgresql/Reveries.Infrastructure.Postgresql.csproj", "Reveries.Infrastructure.Postgresql/"]
COPY ["src/Reveries.Infrastructure.Redis/Reveries.Infrastructure.Redis.csproj", "Reveries.Infrastructure.Redis/"]
COPY ["src/Reveries.Integration/Reveries.Integration.csproj", "Reveries.Integration/"]
COPY ["src/Reveries.Integration.GoogleBooks/Reveries.Integration.GoogleBooks.csproj", "Reveries.Integration.GoogleBooks/"]
COPY ["src/Reveries.Integration.Isbndb/Reveries.Integration.Isbndb.csproj", "Reveries.Integration.Isbndb/"]

RUN dotnet restore "Reveries.Api/Reveries.Api.csproj"

COPY src/ .

WORKDIR "/src/Reveries.Api"

RUN dotnet publish "Reveries.Api.csproj"  \
    -c Release  \
    -o /app/publish  \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Reveries.Api.dll"]