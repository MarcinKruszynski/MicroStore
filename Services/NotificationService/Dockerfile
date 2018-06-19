FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Services/NotificationService/NotificationService.csproj Services/NotificationService/
RUN dotnet restore Services/NotificationService/NotificationService.csproj
COPY . .
WORKDIR /src/Services/NotificationService
RUN dotnet build NotificationService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish NotificationService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
