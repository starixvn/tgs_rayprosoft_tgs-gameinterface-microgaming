#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tgs.GameInterface.MicroGaming/Tgs.GameInterface.MicroGaming.csproj", "Tgs.GameInterface.MicroGaming/"]
COPY ["Tgs.GameInterface.MicroGaming.Core/Tgs.GameInterface.MicroGaming.Core.csproj", "Tgs.GameInterface.MicroGaming.Core/"]
RUN dotnet restore "./Tgs.GameInterface.MicroGaming/Tgs.GameInterface.MicroGaming.csproj"
COPY . .
WORKDIR "/src/Tgs.GameInterface.MicroGaming"
RUN dotnet build "./Tgs.GameInterface.MicroGaming.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameInterface.MicroGaming.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameInterface.MicroGaming.dll"]