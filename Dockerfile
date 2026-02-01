# Build React client app
FROM node:20-alpine AS client-build
WORKDIR /src
COPY ["clientapp/package.json", "clientapp/package-lock.json", "./clientapp/"]
RUN cd clientapp && npm ci
COPY clientapp/ ./clientapp/
WORKDIR /src/clientapp
RUN npm run build
# Verify wwwroot was created
RUN ls -la /src/wwwroot || (echo "wwwroot not found!" && exit 1)

# Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyEmailAliasesApi.csproj", "./"]
RUN dotnet restore
COPY . .
# Copy built React app from client-build stage
COPY --from=client-build /src/wwwroot ./wwwroot
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Explicitly copy wwwroot
COPY --from=build /src/wwwroot ./wwwroot
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "MyEmailAliasesApi.dll"]