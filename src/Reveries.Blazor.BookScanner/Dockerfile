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

RUN printf 'events {}\n\
http {\n\
    include /etc/nginx/mime.types;\n\
    server {\n\
        listen 80;\n\
        location / {\n\
            root /usr/share/nginx/html;\n\
            try_files $uri $uri/ /index.html;\n\
        }\n\
    }\n\
}\n' > /etc/nginx/nginx.conf

EXPOSE 80