FROM mcr.microsoft.com/dotnet/sdk:5.0.404-alpine3.14 AS base
WORKDIR /app
RUN apk add --update openssl && \
    rm -rf /var/cache/apk/*

FROM mcr.microsoft.com/dotnet/sdk:5.0.404-alpine3.14 as build
WORKDIR /src
COPY ["IntegrationTest/IntegrationTest.csproj", "IntegrationTest/"]
RUN dotnet restore "IntegrationTest/IntegrationTest.csproj"

COPY ["IntegrationTest/", "IntegrationTest/"]
RUN dotnet publish "IntegrationTest/IntegrationTest.csproj" -c Release -o /app

FROM base AS final
COPY --from=build /app .
CMD ["dotnet", "test", "IntegrationTest.dll", "--nologo", "-v", "n"]
