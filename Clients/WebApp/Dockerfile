FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0.5-2.1.4 AS build
WORKDIR /src
COPY Clients/WebApp/WebApp.csproj Clients/WebApp/
RUN dotnet restore Clients/WebApp/
COPY . .
WORKDIR /src/Clients/WebApp
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebApp.dll"]
