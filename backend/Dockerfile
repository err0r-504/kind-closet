# Этап 1: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл решения и проекта (для кэширования слоев Docker)
COPY kind-closet.sln ./
COPY src/kind-closet.csproj ./src/
RUN dotnet restore "src/kind-closet.csproj"

# Копируем весь остальной исходный код и компилируем
COPY src/. ./src/
WORKDIR /src/src
RUN dotnet publish "kind-closet.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап 2: Среда выполнения (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Стандартный порт для ASP.NET Core 8 в Docker
EXPOSE 8080 

ENTRYPOINT ["dotnet", "kind-closet.dll"]