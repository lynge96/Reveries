FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Reveries.Blazor.BookScanner/Reveries.Blazor.BookScanner.csproj", "Reveries.Blazor.BookScanner/"]
COPY ["src/Reveries.Contracts/Reveries.Contracts.csproj", "Reveries.Contracts/"]

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "Reveries.Blazor.BookScanner/Reveries.Blazor.BookScanner.csproj"

COPY src/ .

WORKDIR "/src/Reveries.Blazor.BookScanner"

RUN dotnet publish "Reveries.Blazor.BookScanner.csproj" \
    -c Release \
    -o /app/publish

FROM nginx:alpine

WORKDIR /usr/share/nginx/html

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY src/Reveries.Blazor.BookScanner/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80