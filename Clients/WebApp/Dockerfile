FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base

# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup

WORKDIR /app
EXPOSE 64969
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk AS build

# Setup NodeJs
RUN apt-get update && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get install -y build-essential nodejs
# End setup

WORKDIR /src
COPY Clients/WebApp/WebApp.csproj Clients/WebApp/
COPY Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/Microsoft.AspNetCore.HealthChecks.csproj Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/
COPY Common/HealthChecks/Microsoft.Extensions.HealthChecks/Microsoft.Extensions.HealthChecks.csproj Common/HealthChecks/Microsoft.Extensions.HealthChecks/

RUN dotnet restore Clients/WebApp/WebApp.csproj
COPY . .
WORKDIR /src/Clients/WebApp
RUN dotnet build WebApp.csproj -c Release -o /app

FROM build AS publish
WORKDIR /src/Clients/WebApp/ClientApp
RUN npm rebuild node-sass --force
WORKDIR /src/Clients/WebApp
RUN dotnet publish WebApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApp.dll"]
