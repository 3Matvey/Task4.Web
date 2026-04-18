FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Task4.Web/Task4.Web.csproj", "Task4.Web/"]
RUN dotnet restore "./Task4.Web/Task4.Web.csproj"

COPY . .
WORKDIR "/src/Task4.Web"
RUN dotnet publish "./Task4.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Task4.Web.dll"]