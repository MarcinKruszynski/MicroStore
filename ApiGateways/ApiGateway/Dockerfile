FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0.5-2.1.4 AS build
WORKDIR /src
COPY ApiGateways/ApiGateway/ApiGateway.csproj ApiGateways/ApiGateway/
RUN dotnet restore ApiGateways/ApiGateway/
COPY . .
WORKDIR /src/ApiGateways/ApiGateway
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
