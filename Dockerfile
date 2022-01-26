#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache --virtual build-deps musl-dev gcc &&\
    apk add --no-cache py3-pip &&\
    pip install --upgrade yt-dlp &&\
    apk del build-deps

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LiveChatToDiscord.csproj", "."]
RUN dotnet restore "./LiveChatToDiscord.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LiveChatToDiscord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LiveChatToDiscord.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV VIDEOID=
ENTRYPOINT ["dotnet", "LiveChatToDiscord.dll"]