#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache --virtual build-deps musl-dev gcc g++ python3-dev &&\
    apk add --no-cache py3-pip tzdata &&\
    pip install yt-dlp &&\
    apk del build-deps
ENV TZ=Asia/Taipei

# Disable file locking on Unix
# https://github.com/dotnet/runtime/issues/34126#issuecomment-1104981659
ENV DOTNET_SYSTEM_IO_DISABLEFILELOCKING=true

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["YoutubeLiveChatToDiscord.csproj", "."]
RUN dotnet restore "YoutubeLiveChatToDiscord.csproj"
COPY . .
RUN dotnet build "YoutubeLiveChatToDiscord.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "YoutubeLiveChatToDiscord.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app && chown -R app:app /app
USER app
ENTRYPOINT ["dotnet", "YoutubeLiveChatToDiscord.dll"]