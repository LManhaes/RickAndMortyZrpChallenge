# Etapa de build com .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o .csproj e restaura
COPY RickAndMortyZrpChallenge/RickAndMortyZrpChallenge.csproj ./RickAndMortyZrpChallenge/
RUN dotnet restore RickAndMortyZrpChallenge/RickAndMortyZrpChallenge.csproj

# Copia o restante e publica
COPY . ./
RUN dotnet publish RickAndMortyZrpChallenge/RickAndMortyZrpChallenge.csproj -c Release -o /app/out /p:UseAppHost=false

# Etapa de runtime com ASP.NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Mantém o container ouvindo na porta 80 (igual ao seu Dockerfile base)
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "RickAndMortyZrpChallenge.dll"]
