#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache tzdata python3 && \
    apk add --no-cache --virtual build-deps musl-dev gcc g++ python3-dev py3-pip && \
    python3 -m venv /venv && \
    source /venv/bin/activate && \
    pip install yt-dlp && \
    pip uninstall -y setuptools pip && \
    apk del build-deps

ENV PATH="/venv/bin:$PATH"
ENV TZ=Asia/Taipei

# Disable file locking on Unix
# https://github.com/dotnet/runtime/issues/34126#issuecomment-1104981659
ENV DOTNET_SYSTEM_IO_DISABLEFILELOCKING=true

FROM base AS debug

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["YoutubeLiveChatToDiscord.csproj", "."]
ARG TARGETPLATFORM
RUN dotnet restore "YoutubeLiveChatToDiscord.csproj"

FROM build AS publish
COPY . .
ARG BUILD_CONFIGURATION=Release
ARG TARGETPLATFORM
RUN dotnet publish "YoutubeLiveChatToDiscord.csproj" --no-self-contained -p:PublishTrimmed=false -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown -R 1001:1001 /app
USER 1001
ENTRYPOINT ["dotnet", "YoutubeLiveChatToDiscord.dll"]