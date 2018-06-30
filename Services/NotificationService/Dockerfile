FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY MicroStore.sln ./
COPY Services/NotificationService/NotificationService.csproj Services/NotificationService/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Services/NotificationService
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NotificationService.dll"]
