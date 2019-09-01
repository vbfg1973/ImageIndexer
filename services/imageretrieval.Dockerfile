FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ImageRetrieval/ImageRetrieval.csproj", "ImageRetrieval/"]
COPY ["Shared/Core/Core.csproj", "Shared/Core/"]
COPY ["Shared/Infrastructure/Infrastructure.csproj", "Shared/Infrastructure/"]
RUN dotnet restore "./ImageRetrieval/ImageRetrieval.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "./ImageRetrieval/ImageRetrieval.csproj" -c Debug -o /app

FROM build AS publish

RUN dotnet publish "./ImageRetrieval/ImageRetrieval.csproj" -c Debug -o /app

FROM build AS final
WORKDIR /app
COPY --from=publish /app .
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*
ENTRYPOINT ["dotnet", "./ImageRetrieval.dll"]