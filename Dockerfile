FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["./Fintrox/Fintrox.csproj", "./Fintrox/"]

RUN dotnet restore "./Fintrox/Fintrox.csproj"
COPY . .

WORKDIR "/src/Fintrox"
RUN dotnet build "Fintrox.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fintrox.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fintrox.dll"]
