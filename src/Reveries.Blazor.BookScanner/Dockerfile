FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Reveries.Blazor.BookScanner/Reveries.Blazor.BookScanner.csproj", "Reveries.Blazor.BookScanner/"]
COPY ["src/Reveries.Contracts/Reveries.Contracts.csproj", "Reveries.Contracts/"]

RUN dotnet restore "Reveries.Blazor.BookScanner/Reveries.Blazor.BookScanner.csproj"

COPY src/ .

WORKDIR "/src/Reveries.Blazor.BookScanner"

RUN dotnet publish "Reveries.Blazor.BookScanner.csproj" \
    -c Release \
    -o /app/publish

FROM nginx:alpine

WORKDIR /usr/share/nginx/html

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

RUN sed -i 's|<base href="/" />|<base href="/scanner/" />|' /usr/share/nginx/html/index.html

EXPOSE 80