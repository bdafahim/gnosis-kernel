FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GnosisKernel.Api/GnosisKernel.Api.csproj", "GnosisKernel.Api/"]
RUN dotnet restore "GnosisKernel.Api/GnosisKernel.Api.csproj"
COPY . .
WORKDIR "/src/GnosisKernel.Api"
RUN dotnet build "./GnosisKernel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GnosisKernel.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GnosisKernel.Api.dll"]
