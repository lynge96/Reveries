FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Reveries.Blazor.BookScanner/Reveries.Blazor.BookScanner.csproj", "Reveries.Blazor.BookScanner/"]
COPY ["src/Reveries.Contracts/Reveries.Contracts.csproj", "Reveries.Contracts/"]

RUN dotnet restore "Reveries.Blazor.BookScanner/Reveries.Blazor.BookScanner.csproj"

COPY src/ .

WORKDIR "/src/Reveries.Blazor.BookScanner"

RUN dotnet publish "Reveries.Blazor.BookScanner.csproj"  \
    -c Release  \
    -o /app/publish  \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Reveries.Blazor.BookScanner.dll"]
