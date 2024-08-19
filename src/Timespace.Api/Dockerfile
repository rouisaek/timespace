FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Timespace.Api/Timespace.Api.csproj", "Timespace.Api/"]
RUN dotnet restore "Timespace.Api/Timespace.Api.csproj"
COPY . .
WORKDIR "/src/Timespace.Api"
RUN dotnet build "Timespace.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Timespace.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#
# Node build
#
FROM node:21 as build-frontend
WORKDIR /source

ENV NPM_CONFIG_UPDATE_NOTIFIER=false
RUN npm i -g --no-audit --no-fund pnpm

COPY ../../frontend src/Web
#COPY ../../frontend/.npmrc .
#COPY ../../frontend/pnpm* .
#COPY ../../frontend/package.json .
#COPY ../../frontend/tsconfig* .

RUN <<EOF
pnpm install
pnpm -r build
EOF

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build-frontend /source/src/Web/dist wwwroot
ENTRYPOINT ["dotnet", "Timespace.Api.dll"]
