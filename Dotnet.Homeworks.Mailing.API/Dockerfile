FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Dotnet.Homeworks.Mailing.API/Dotnet.Homeworks.Mailing.API.csproj", "Dotnet.Homeworks.Mailing.API/"]
COPY ["Dotnet.Homeworks.Shared/Dotnet.Homeworks.Shared.csproj", "Dotnet.Homeworks.Shared/"]
RUN dotnet restore "Dotnet.Homeworks.Mailing.API/Dotnet.Homeworks.Mailing.API.csproj"
COPY . .
WORKDIR "/src/Dotnet.Homeworks.Mailing.API"
RUN dotnet build "Dotnet.Homeworks.Mailing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dotnet.Homeworks.Mailing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dotnet.Homeworks.Mailing.API.dll"]
