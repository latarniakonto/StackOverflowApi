
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StackOverflow.Api/StackOverflow.Api.csproj", "StackOverflow.Api/"]
COPY . .
WORKDIR "/src/StackOverflow.Api"
RUN dotnet build "StackOverflow.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StackOverflow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StackOverflow.Api.dll"]
