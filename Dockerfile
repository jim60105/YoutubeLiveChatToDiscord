#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache --virtual build-deps musl-dev gcc g++ python3-dev &&\
    apk add --no-cache py3-pip tzdata &&\
    pip install --upgrade yt-dlp &&\
    apk del build-deps
ENV TZ=Asia/Taipei

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["YoutubeLiveChatToDiscord.csproj", "."]
RUN dotnet restore "./YoutubeLiveChatToDiscord.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "YoutubeLiveChatToDiscord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YoutubeLiveChatToDiscord.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YoutubeLiveChatToDiscord.dll"]