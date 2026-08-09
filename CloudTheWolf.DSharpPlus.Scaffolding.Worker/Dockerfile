FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

# The Worker transitively requires Microsoft.AspNetCore.App, while retaining the
# smaller general-purpose .NET runtime image as its base.
COPY --from=mcr.microsoft.com/dotnet/aspnet:10.0 \
    /usr/share/dotnet/shared/Microsoft.AspNetCore.App \
    /usr/share/dotnet/shared/Microsoft.AspNetCore.App

# Install runtime dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libgdiplus \
        fonts-freefont-ttf \
        libssl3 \
        libc6-dev \
        libx11-dev \
        libx11-6 \
        libxext-dev \
        libxext6 \
        libxrender1 \
        fontconfig \
        libfreetype6-dev \
        libpng-dev \
        imagemagick \
    && rm -rf /var/lib/apt/lists/*

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project from the current directory
COPY . .

# Restore and publish the csproj in the current directory
RUN dotnet restore ./*.csproj

RUN dotnet publish ./*.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV FONTCONFIG_PATH=/etc/fonts

ENTRYPOINT ["dotnet", "CloudTheWolf.DSharpPlus.Scaffolding.Worker.dll"]