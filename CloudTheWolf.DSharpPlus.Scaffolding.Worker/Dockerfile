# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# Install necessary packages
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    libgdiplus \
    fonts-freefont-ttf \
    libssl1.1 \
    libc6-dev \
    libx11-dev \
    libx11-6 \
    libxext-dev \
    libxext6 \
    libxrender1 \
    fontconfig \
    libfreetype6-dev \
    libpng-dev && \
    rm -rf /var/lib/apt/lists/* && \
    curl -O http://ftp.us.debian.org/debian/pool/main/i/imagemagick/imagemagick_6.9.11.60+dfsg-1.3+deb11u3_amd64.deb && \
    dpkg -i imagemagick_6.9.11.60+dfsg-1.3+deb11u3_amd64.deb

FROM base AS final
WORKDIR /app
COPY ./publish .
ENV FONTCONFIG_PATH=/etc/fonts
ENTRYPOINT ["dotnet", "CloudTheWolf.DSharpPlus.Scaffolding.Worker.dll"]
