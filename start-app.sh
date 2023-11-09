docker compose up -d
sleep 5
dotnet run --launch-profile Localhost --project ./src/Chirp.Web/Chirp.Web.csproj
