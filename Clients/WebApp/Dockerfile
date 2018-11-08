FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base

# Setup NodeJs
RUN apt-get update -qq && \
    apt-get install -qq -y wget && \
    apt-get install -qq -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_8.x | bash - && \
    apt-get install -qq -y build-essential nodejs 
# End setup

WORKDIR /app
EXPOSE 64969
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk AS build

# Setup NodeJs
RUN apt-get update -qq && \
    apt-get install -qq -y wget && \
    apt-get install -qq -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_8.x | bash - && \
    apt-get install -qq -y build-essential nodejs 
# End setup

WORKDIR /src
COPY Clients/WebApp/WebApp.csproj Clients/WebApp/
COPY Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/Microsoft.AspNetCore.HealthChecks.csproj Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/
COPY Common/HealthChecks/Microsoft.Extensions.HealthChecks/Microsoft.Extensions.HealthChecks.csproj Common/HealthChecks/Microsoft.Extensions.HealthChecks/
RUN dotnet restore Clients/WebApp/WebApp.csproj

COPY Clients/WebApp/ClientApp/package.json Clients/WebApp/ClientApp/

RUN cd Clients/WebApp/ClientApp \
    && npm i --silent

COPY . .
WORKDIR /src/Clients/WebApp
RUN dotnet build WebApp.csproj -c Release -o /app

FROM build AS publish
WORKDIR /src/Clients/WebApp
RUN dotnet publish WebApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApp.dll"]
