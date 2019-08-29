FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ImageRetrieval/ImageRetrieval.csproj", "ImageRetrieval/"]
RUN dotnet restore "./ImageRetrieval/ImageRetrieval.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "./ImageRetrieval/ImageRetrieval.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "./ImageRetrieval/ImageRetrieval.csproj" -c Debug -o /app

FROM build AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "./ImageRetrieval.dll"]