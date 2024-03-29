FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["RedditListener/RedditListener.csproj", "RedditListener/"]
COPY ["Shared/Core/Core.csproj", "Shared/Core/"]
COPY ["Shared/Infrastructure/Infrastructure.csproj", "Shared/Infrastructure/"]
RUN dotnet restore "./RedditListener/RedditListener.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "./RedditListener/RedditListener.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "./RedditListener/RedditListener.csproj" -c Debug -o /app

FROM build AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "./RedditListener.dll"]