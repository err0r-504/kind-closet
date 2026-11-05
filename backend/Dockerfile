# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# Копируем файл проекта из подпапки src
COPY src/kind-closet.csproj ./src/
RUN dotnet restore src/kind-closet.csproj

# Копируем остальные файлы и компилируем
COPY src/ ./src/
WORKDIR /app/src
RUN dotnet publish kind-closet.csproj -c Release -o /app/publish /p:UseAppHost=false

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "kind-closet.dll"]