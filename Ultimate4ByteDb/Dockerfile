FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["Server/Ultimate4ByteDb.Server.csproj", "Server/"]
RUN dotnet restore "Server/Ultimate4ByteDb.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Ultimate4ByteDb.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ultimate4ByteDb.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ultimate4ByteDb.Server.dll"]
