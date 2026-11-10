# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copia y restaura las dependencias
COPY . ./
RUN dotnet restore StockTiendas/StockTiendas.csproj

# Compilar la aplicación
RUN dotnet publish StockTiendas/StockTiendas.csproj -c Release -o /out --no-restore

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Configurar variable de entorno
ENV ASPNETCORE_ENVIRONMENT=Production

# Copia los archivos compilados de la etapa anterior
COPY --from=build /out ./

# Expone el puerto de la API
EXPOSE 8080

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "StockTiendas.dll"]
