#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Riccardos77.AppConfig.Api.sln", "."]
COPY ["AppConfig.Abstractions/Riccardos77.AppConfig.Abstractions.csproj", "./AppConfig.Abstractions/"]
COPY ["AppConfig.Api/Riccardos77.AppConfig.Api.csproj", "./AppConfig.Api/"]
COPY ["AppConfig.DataAccessProviders.AzureBlobStorage/Riccardos77.AppConfig.DataAccessProviders.AzureBlobStorage.csproj", "./AppConfig.DataAccessProviders.AzureBlobStorage/"]
RUN dotnet restore "./Riccardos77.AppConfig.Api.sln"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Riccardos77.AppConfig.Api.sln" -c Release -o /app/build
RUN dotnet publish "Riccardos77.AppConfig.Api.sln" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Riccardos77.AppConfig.Api.dll"]
