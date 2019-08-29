FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ImageApi/ImageApi.csproj", "ImageApi/"]
RUN dotnet restore "./ImageApi/ImageApi.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "./ImageApi/ImageApi.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "./ImageApi/ImageApi.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "./ImageApi.dll"]