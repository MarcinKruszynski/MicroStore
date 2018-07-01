FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 64969
EXPOSE 44383

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Services/ProductService/ProductService.csproj Services/ProductService/
COPY Common/RabbitMQ.Common/RabbitMQ.Common.csproj Common/RabbitMQ.Common/
RUN dotnet restore Services/ProductService/ProductService.csproj
COPY . .
WORKDIR /src/Services/ProductService
RUN dotnet build ProductService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ProductService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ProductService.dll"]
