#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Orders/Orders.API/Orders.API.csproj", "src/Orders/Orders.API/"]
COPY ["src/Orders/Orders.Infrastructure/Orders.Infrastructure.csproj", "src/Orders/Orders.Infrastructure/"]
COPY ["src/Orders/Orders.Core/Orders.Core.csproj", "src/Orders/Orders.Core/"]
COPY ["src/Common/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "src/Common/EventBusRabbitMQ/"]
COPY ["src/Orders/Orders.Application/Orders.Application.csproj", "src/Orders/Orders.Application/"]
RUN dotnet restore "src/Orders/Orders.API/Orders.API.csproj"
COPY . .
WORKDIR "/src/src/Orders/Orders.API"
RUN dotnet build "Orders.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Orders.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.API.dll"]
