dotnet ef migrations add InitialMigrations --project ./src/Chirp.Infrastructure/ --startup-project ./src/Chirp.Web/
docker compose -f compose.yaml -p chirp up -d db
sleep 5
dotnet ef database update --project=./src/Chirp.Web/Chirp.Web.csproj
rm -rf ./src/Chirp.Infrastructure/Migrations
docker compose stop
