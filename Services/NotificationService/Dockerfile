FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 64969
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Services/NotificationService/NotificationService.csproj Services/NotificationService/
COPY Common/RabbitMQ.Common/RabbitMQ.Common.csproj Common/RabbitMQ.Common/
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
