FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 7044

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY DockerProxy/DockerProxy.csproj DockerProxy/
RUN dotnet restore DockerProxy/DockerProxy.csproj
COPY . .
WORKDIR /src/DockerProxy
RUN dotnet build DockerProxy.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish DockerProxy.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DockerProxy.dll"]
