FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 64969
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Monitoring/WebStatus/WebStatus.csproj Monitoring/WebStatus/
COPY Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/Microsoft.AspNetCore.HealthChecks.csproj Common/HealthChecks/Microsoft.AspNetCore.HealthChecks/
COPY Common/HealthChecks/Microsoft.Extensions.HealthChecks/Microsoft.Extensions.HealthChecks.csproj Common/HealthChecks/Microsoft.Extensions.HealthChecks/

RUN dotnet restore Monitoring/WebStatus/WebStatus.csproj
COPY . .
WORKDIR /src/Monitoring/WebStatus
RUN dotnet build WebStatus.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebStatus.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]
