FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 64969
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Services/NotificationService/NotificationService.csproj Services/NotificationService/
COPY Common/RabbitMQ.Common/RabbitMQ.Common.csproj Common/RabbitMQ.Common/
COPY Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/Microsoft.AspNetCore.HealthChecks.csproj Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/
COPY Common/HealthChecks/Microsoft.Extensions.HealthChecks/Microsoft.Extensions.HealthChecks.csproj Common/HealthChecks/Microsoft.Extensions.HealthChecks/
COPY Common/HealthChecks/MicroStore.Extensions.HealthChecks.PostgreSQL/MicroStore.Extensions.HealthChecks.PostgreSQL.csproj Common/HealthChecks/MicroStore.Extensions.HealthChecks.PostgreSQL/

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
