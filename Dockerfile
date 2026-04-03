# 1. Etapa de compilación (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar archivos del proyecto y restaurar dependencias
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

# Copiar todo lo demás y compilar
COPY . ./
RUN dotnet publish -c Release -o out

# 2. Etapa de ejecución (Runtime más ligero)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configurar el puerto para Render
# Render asigna un puerto dinámico mediante la variable de entorno PORT
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Cursos_AI_Back.dll"]