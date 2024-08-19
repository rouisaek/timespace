# syntax=docker/dockerfile:latest
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Timespace.Api/Timespace.Api.csproj", "src/Timespace.Api/"]
COPY ["./Directory.Build.props", "./"]
COPY ["./Directory.Packages.props", "./"]
COPY ["src/Timespace.SourceGenerators/Timespace.SourceGenerators.csproj", "src/Timespace.SourceGenerators/"]
COPY ["src/Timespace.Analyzers/Timespace.Analyzers.csproj", "src/Timespace.Analyzers/"]
RUN dotnet restore "src/Timespace.Api/Timespace.Api.csproj"
COPY . .
WORKDIR "/src/src/Timespace.Api"
RUN dotnet build "Timespace.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Timespace.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:21 as build-stage
WORKDIR /build
RUN npm install -g pnpm

COPY ./frontend/package.json .
COPY ./frontend/pnpm* .

RUN pnpm install
COPY ./frontend .
RUN pnpm run build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build-stage /build/dist ./wwwroot
ENTRYPOINT ["dotnet", "Timespace.Api.dll"]
