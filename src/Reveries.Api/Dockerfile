FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Reveries.Api/Reveries.Api.csproj", "Reveries.Api/"]
COPY ["src/Reveries.Application/Reveries.Application.csproj", "Reveries.Application/"]
COPY ["src/Reveries.Domain/Reveries.Domain.csproj", "Reveries.Domain/"]
COPY ["src/Reveries.Contracts/Reveries.Contracts.csproj", "Reveries.Contracts/"]
COPY ["src/Reveries.Infrastructure/Reveries.Infrastructure.csproj", "Reveries.Infrastructure/"]
COPY ["src/Reveries.Persistence/Reveries.Persistence.csproj", "Reveries.Persistence/"]
COPY ["src/Reveries.Integration/Reveries.Integration.csproj", "Reveries.Integration/"]

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