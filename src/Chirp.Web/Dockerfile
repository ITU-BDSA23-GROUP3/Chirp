FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
#RUN dotnet tool install --global dotnet-ef

COPY ["src/Chirp.Web/Chirp.Web.csproj", "src/Chirp.Web/"]
COPY ["src/Chirp.Core/Chirp.Core.csproj", "src/Chirp.Core/"]
COPY ["src/Chirp.Infrastructure/Chirp.Infrastructure.csproj", "src/Chirp.Infrastructure/"]
RUN dotnet restore "src/Chirp.Web/Chirp.Web.csproj"
COPY . .
WORKDIR "/src/src/Chirp.Web"
#RUN dotnet ef database update
RUN dotnet build "Chirp.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chirp.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chirp.Web.dll", "--environment=Development"]
