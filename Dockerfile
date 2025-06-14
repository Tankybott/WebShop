# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WebShop.sln ./
COPY WebShop/WebShop.csproj ./WebShop/
COPY Models/Models.csproj ./Models/
COPY DataAccess/DataAccess.csproj ./DataAccess/
COPY ControllersServices/Services.csproj ./ControllersServices/
COPY Utility/Utility.csproj ./Utility/
COPY BackgroundServices/BackgroundServices.csproj ./BackgroundServices/

RUN dotnet restore
COPY . .

# Build in Release and publish output to /publish
RUN dotnet publish WebShop/WebShop.csproj -c Release -o /publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install native dependencies required by wkhtmltox
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libxrender1 \
    libxext6 \
    libx11-6 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy published app
COPY --from=build /publish ./

# Copy libwkhtmltox.so from bin/Release/net8.0 into /app
COPY --from=build /src/WebShop/bin/Release/net8.0/libwkhtmltox.so /app/libwkhtmltox.so

# Let the system find the native lib
ENV LD_LIBRARY_PATH=/app

# Change to Production mode
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "WebShop.dll"]
