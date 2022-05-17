#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Riccardos77.AppConfig.sln", "."]
COPY ["Riccardos77.AppConfig.Abstractions/Riccardos77.AppConfig.Abstractions.csproj", "./Riccardos77.AppConfig.Abstractions/"]
COPY ["Riccardos77.AppConfig.Api/Riccardos77.AppConfig.Api.csproj", "./Riccardos77.AppConfig.Api/"]
COPY ["Riccardos77.AppConfig.Core/Riccardos77.AppConfig.Core.csproj", "./Riccardos77.AppConfig.Core/"]
COPY ["Riccardos77.AppConfig.DataProviders.AzureBlobStorage/Riccardos77.AppConfig.DataProviders.AzureBlobStorage.csproj", "./Riccardos77.AppConfig.DataProviders.AzureBlobStorage/"]
RUN dotnet restore "./Riccardos77.AppConfig.sln"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Riccardos77.AppConfig.sln" -c Release -o /app/build
RUN dotnet publish "Riccardos77.AppConfig.sln" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Riccardos77.AppConfig.Api.dll"]
