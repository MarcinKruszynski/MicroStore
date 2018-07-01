FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 64969
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Services/IdentityService/IdentityService.csproj Services/IdentityService/
COPY Common/WebHost.Common/WebHost.Common.csproj Common/WebHost.Common/
RUN dotnet restore Services/IdentityService/IdentityService.csproj
COPY . .
WORKDIR /src/Services/IdentityService
RUN dotnet build IdentityService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IdentityService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityService.dll"]
