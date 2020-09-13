FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS web
LABEL maintainer 'Wy7 <wythorbazan@gmail.com>'
# Seta um diretório de trabalho para o nosso app.
WORKDIR /app

# Copia o projeto e da um restore dentro do container.
COPY *.csproj ./
RUN dotnet restore

# Copia o restante, e também publica.
COPY . ./
RUN dotnet publish -c Release -o out

# Builda uma imagem de RunTime.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
WORKDIR /app
# Copia o que ta na imagem do app para a imagem do runtime.
COPY --from=web /app/out .
EXPOSE 80
# Seta um local de execução assim que o container for iniciado.
ENTRYPOINT ["dotnet", "Order.dll"]
