FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 28413
EXPOSE 44377

FROM microsoft/dotnet:2.1-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY Aniel.ConnectAPI/Aniel.ConnectAPI.csproj Aniel.Claro/
RUN dotnet restore Aniel.ConnectAPI/Aniel.Claro.csproj
COPY . .
WORKDIR /src/Aniel.ConnectAPI
RUN dotnet build Aniel.ConnectAPI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Aniel.ConnectAPI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Aniel.ConnectAPI.dll"]
