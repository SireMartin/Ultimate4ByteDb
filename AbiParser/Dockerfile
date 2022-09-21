FROM mcr.microsoft.com/dotnet/sdk:6.0-focal as build-env
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0-focal
ENV SOURCE_PATH=/data
ENV REDIS_CONNSTR=redis:6379,abortConnect=false,ssl=false,allowAdmin=true
RUN mkdir /data
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "AbiParser.dll"]