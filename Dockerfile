### Debug image
### Setup the same as base image but used dotnet/runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS debug

WORKDIR /app
RUN apk add --no-cache tzdata python3 && \
    apk add --no-cache --virtual build-deps musl-dev gcc g++ python3-dev py3-pip && \
    python3 -m venv /venv && \
    source /venv/bin/activate && \
    pip install --no-cache-dir yt-dlp && \
    pip uninstall -y setuptools pip && \
    apk del build-deps

ENV PATH="/venv/bin:$PATH"
ENV TZ=Asia/Taipei

# Disable file locking on Unix
# https://github.com/dotnet/runtime/issues/34126#issuecomment-1104981659
ENV DOTNET_SYSTEM_IO_DISABLEFILELOCKING=true

### Base image for yt-dlp
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache tzdata python3 && \
    apk add --no-cache --virtual build-deps musl-dev gcc g++ python3-dev py3-pip && \
    python3 -m venv /venv && \
    source /venv/bin/activate && \
    pip install --no-cache-dir yt-dlp && \
    pip uninstall -y setuptools pip && \
    apk del build-deps

ENV PATH="/venv/bin:$PATH"
ENV TZ=Asia/Taipei

# Disable file locking on Unix
# https://github.com/dotnet/runtime/issues/34126#issuecomment-1104981659
ENV DOTNET_SYSTEM_IO_DISABLEFILELOCKING=true

### Build .NET
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src

COPY ["YoutubeLiveChatToDiscord.csproj", "."]
RUN dotnet restore -a $TARGETARCH "YoutubeLiveChatToDiscord.csproj"

FROM build AS publish
COPY . .
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
RUN dotnet publish "YoutubeLiveChatToDiscord.csproj" -a $TARGETARCH -c $BUILD_CONFIGURATION -o /app/publish --self-contained true


### Final image
FROM base AS final

ENV PATH="/app:$PATH"

RUN mkdir -p /app && chown -R $APP_UID:$APP_UID /app && chmod u+rwx /app
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish/YoutubeLiveChatToDiscord /app/YoutubeLiveChatToDiscord
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish/appsettings.json /app/appsettings.json

USER $APP_UID
ENTRYPOINT ["/app/YoutubeLiveChatToDiscord"]