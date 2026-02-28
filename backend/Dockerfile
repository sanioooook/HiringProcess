# Build 
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy solution and project files first (layer-cache friendly)
COPY HiringProcess.slnx ./
COPY src/HiringProcess.Api/HiringProcess.Api.csproj src/HiringProcess.Api/
COPY tests/HiringProcess.Tests/HiringProcess.Tests.csproj tests/HiringProcess.Tests/

RUN dotnet restore HiringProcess.slnx

# Copy full source
COPY . .

# Run tests
RUN dotnet test tests/HiringProcess.Tests/HiringProcess.Tests.csproj \
    --no-restore \
    --configuration Release \
    --logger "console;verbosity=minimal"

# Publish the API
RUN dotnet publish src/HiringProcess.Api/HiringProcess.Api.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app

# Create uploads directory and set ownership before switching to non-root user
RUN mkdir -p /app/uploads && chown -R $APP_UID /app/uploads

COPY --from=build /app/publish .

# .NET runtime images define $APP_UID (pre-created non-root user)
USER $APP_UID

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "HiringProcess.Api.dll"]
