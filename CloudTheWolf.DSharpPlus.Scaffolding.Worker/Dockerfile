ARG IMAGEMAGICK_VERSION=7.1.2-29
ARG IMAGEMAGICK_SHA256=b05924ad73c6932ba62c9b32f338f0619b90b767162c8b767f3566556187a284

FROM alpine:3.24 AS imagemagick-build
ARG IMAGEMAGICK_VERSION
ARG IMAGEMAGICK_SHA256

RUN apk add --no-cache \
        build-base \
        curl \
        fontconfig-dev \
        freetype-dev \
        libjpeg-turbo-dev \
        libpng-dev \
        libwebp-dev \
        libxml2-dev \
        pkgconf \
        tiff-dev \
        xz \
        zlib-dev

WORKDIR /tmp/imagemagick
RUN curl --fail --silent --show-error --location \
        --output imagemagick.tar.gz \
        "https://github.com/ImageMagick/ImageMagick/archive/refs/tags/${IMAGEMAGICK_VERSION}.tar.gz" \
    && echo "${IMAGEMAGICK_SHA256}  imagemagick.tar.gz" | sha256sum -c - \
    && tar --extract --gzip --file imagemagick.tar.gz --strip-components=1 \
    && ./configure \
        --prefix=/usr/local \
        --disable-docs \
        --disable-openmp \
        --disable-static \
        --enable-hdri \
        --enable-shared \
        --with-magick-plus-plus=no \
        --with-modules=no \
        --with-perl=no \
        --with-quantum-depth=16 \
        --with-security-policy=limited \
        --with-x=no \
    && make -j"$(getconf _NPROCESSORS_ONLN)" \
    && make DESTDIR=/imagemagick-root install \
    && rm -rf \
        /imagemagick-root/usr/local/include \
        /imagemagick-root/usr/local/lib/pkgconfig \
    && find /imagemagick-root/usr/local -type f -name '*.la' -delete

FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine3.24 AS base
WORKDIR /app

# The Worker transitively requires Microsoft.AspNetCore.App, while retaining the
# smaller general-purpose .NET runtime image as its base.
COPY --from=mcr.microsoft.com/dotnet/aspnet:10.0-alpine3.24 \
    /usr/share/dotnet/shared/Microsoft.AspNetCore.App \
    /usr/share/dotnet/shared/Microsoft.AspNetCore.App

COPY --from=imagemagick-build /imagemagick-root/usr/local /usr/local

# Install runtime dependencies
RUN apk upgrade --no-cache && \
    apk add --no-cache \
        libgdiplus \
        font-freefont \
        libx11 \
        libxext \
        libxrender \
        fontconfig \
        freetype \
        libjpeg-turbo \
        libpng \
        libwebp \
        libwebpdemux \
        libwebpmux \
        libxml2 \
        tiff \
        zlib

RUN magick -version

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine3.24 AS build
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
