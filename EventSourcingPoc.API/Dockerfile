FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /src
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["EventSourcingPoc.API/EventSourcingPoc.API.csproj", "EventSourcingPoc.API/"]
RUN dotnet restore "EventSourcingPoc.API/EventSourcingPoc.API.csproj"

COPY . .
WORKDIR "/src/EventSourcingPoc.API"
RUN dotnet build "EventSourcingPoc.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EventSourcingPoc.API.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "EventSourcingPoc.API.dll" ]