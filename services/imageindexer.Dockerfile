FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ImageIndexer/ImageIndexer.csproj", "ImageIndexer/"]
RUN dotnet restore "./ImageIndexer/ImageIndexer.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "./ImageIndexer/ImageIndexer.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "./ImageIndexer/ImageIndexer.csproj" -c Debug -o /app

FROM build AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "./ImageIndexer.dll"]