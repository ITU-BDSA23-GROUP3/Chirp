# Chirp

# Notes from README Project
Remember to have `Co-authored-by: Name <email@example.com>` in your commit messages. \
Remember KISS - Keep It Simple Stupid. \
Remember to use `git tag` for your release based pushes


# How to run and develop

First you need the dotnet-runtime package, dotnet-sdk and aspnet-runtime. \
You also need Entity Framework core which can be installed after with this command:
```bash
  dotnet tool install --global dotnet-ef --version 7.0.14
```
The next step is to create a [Github oauth app](https://github.com/settings/developers)

Now you need to set your development secrets. \

[user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=linux)

this is the client id name
development:authentication:github:clientId 

this is the client secret name
development:authentication:github:clientId 

Microsoft documentation for setting secrets:
```bash
dotnet user-secrets set "Movies:ServiceApiKey" "12345"
```

Now install docker, and run 
```bash
docker pull mcr.microsoft.com/mssql/server:latest
```
then

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Adsa2023" -p 1433:1433  --name sqlpreview --hostname sqlpreview -d mcr.microsoft.com/mssql/server:2022-latest
```

Go into the src/ directory and run
```bash
dotnet ef migrations add InitialMigrations --project Chirp.Infrastructure/ --startup-project Chirp.Web/
```

Now go into Chirp.Web/ directory and run
```bash
dotnet build
dotnet ef database update
dotnet run --launch-profile Localhost
```

If you did everything correctly it *should work*
If you change the structure of the database, you might want to remove the docker container and the migrations and redeploy it again - just follow the same steps in the same order and it should be good.

