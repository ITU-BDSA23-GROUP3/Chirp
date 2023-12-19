# How to run and develop
First install git and clone this repository
```bash
git clone https://github.com/ITU-BDSA23-GROUP3/Chirp
```

To run the application you need the dotnet-runtime package, dotnet-sdk and aspnet-runtime. \
***Remember to install dotnet 7 and not dotnet 8***
On debian it is done with 
```bash
sudo apt install dotnet-sdk-7.0
```
You also need Entity Framework core which can be installed after with this command:
```bash
  dotnet tool install --global dotnet-ef --version 7.0.14
```
You might need to add dotnet tools to your PATH
Add this line to the bottom of your ~/.bashrc.
```bash
export PATH="$PATH:$HOME/.dotnet/tools/"
```
If you want the changes to take effect in the current terminal immedietly then you can run
```bash
source ~/.bashrc
```

The next step is to create a [Github oauth app](https://github.com/settings/developers)

Click new oauth app and fill in the details. 
Homepage url should be: http://localhost:1339
The callback url should be: http://localhost:1339/signin-github
Client ID is specified on you apps page. And you need to generate a secret on the same page.
You can generate a new secret if you lose it.
Now you need to set your development secrets. 
[user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=linux)

Navigate to /src/Chirp.Web and init your user-secrets with
```bash
dotnet user-secrets init
```

this is the client id name
development:authentication:github:clientId 

this is the client secret name
development:authentication:github:clientSecret

Set secrets with these names
```bash
dotnet user-secrets set "development:authentication:github:clientId" "<client id>"
dotnet user-secrets set "development:authentication:github:clientSecret" "<secret id>"
```

Now install docker, and run 
```bash
docker pull mcr.microsoft.com/mssql/server:latest
```
Remember sudo if you are on linux!
Then you can start the container
We recommend modifying this default password
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

### To run Selenium testcases
We're using Selenium Grid to automate our testing. This makes headless development really easy across platforms. The only dependency needed to use this efficiently is docker, which you'd already need to have to succesfully run it anyways. Specifically you need to run:
```bash
sudo docker run --net="host" -d -p 4444:4444 -v /dev/shm:/dev/shm selenium/standalone-chrome
```
[Note: You may need to run this without `sudo`, and that this might not work out of the gate on non-UNIX compliant systems]

Ensure that the application is running by doing:
```bash
dotnet run --launch-profile Localhost
```

Now navigate to the `test/Chirp.Infrastructure.UnitTest` directory, and run:
```
dotnet test
```
