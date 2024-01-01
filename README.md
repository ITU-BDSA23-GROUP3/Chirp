## How to make _Chirp!_ work locally

### Step One [Prerequisites]
First install `Git` and clone this repository
```bash
git clone https://github.com/ITU-BDSA23-GROUP3/Chirp
```
**[Note: If you are on Windows we recommend using WSL]**

### Step Two [Dependencies]
To run the application you need the `dotnet-runtime` package, `dotnet-sdk` and `aspnet-runtime`. \
**[Note: Remember to install `Dotnet 7` and not a different version]**

On `Debian`-based systems this can be done `apt`: 
```bash
sudo apt install dotnet-sdk-7.0
```
You can also just download it via the [website](https://dotnet.microsoft.com/en-us/download).

You also need Entity Framework Core:
```bash
dotnet tool install --global dotnet-ef --version 7.0.14
```
You might need to add dotnet tools to your PATH. This can be done in bash by adding this line to the bottom of your `~/.bashrc`.
```bash
export PATH="$PATH:$HOME/.dotnet/tools/"
```
Afterwards restart your terminal.

### Step Three [Github OAuth]
Now we will create a [Github OAuth app](https://github.com/settings/developers)

Click `New OAuth app` and fill in the details. The homepage URL should be: `http://localhost:1339` and the callback URL should be `http://localhost:1339/signin-github`.

You should now take note of the `client ID`, you will need this in a second. This can be found on the apps page.

Now you need to generate a secret, take note/copy of the `client secret` as well, this is done on the same page. 


We need this such that we can set our [Development Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=linux).

### Step Four [Secrets]
```bash
dotnet user-secrets --project src/Chirp.Web init 
```

Set secrets with these names
```bash
dotnet user-secrets --project src/Chirp.Web \ 
    set "development:authentication:github:clientId" "<client id>"
dotnet user-secrets --project src/Chirp.Web \
    set "development:authentication:github:clientSecret" "<secret id>" 
```

### Step Five [Running it locally!]
**[Note: Remember to replace <client id> and <secret id> with the respective values from the prior section]**
    
We have set it up to work with Docker which can be downloaded [here](https://www.docker.com/products/docker-desktop/).
    
If you are using a windows machine you can also choose to use SQL server, though you are going to have to change the connection string in `src/Chirp.Web/appsettings.Development.json`.

If you still want to use Docker you have to do the following:

Make sure you have installed `Docker`, opened Docker desktop, and run 
```bash
sudo docker pull mcr.microsoft.com/mssql/server:latest
```
Now you are almost ready! You can run the MsSQL, using the following command:
```bash
sudo docker run \
    -e "ACCEPT_EULA=Y" \
    -e "MSSQL_SA_PASSWORD=Adsa2023" \
    -p 1433:1433 \
    --name sqlpreview \
    --hostname sqlpreview \
    -d mcr.microsoft.com/mssql/server:2022-latest
```
**[Note: We recommend modifying this default password]**

Make sure that you are in the root directory of Chirp and run
```bash
dotnet ef migrations add InitialMigrations \
    --project src/Chirp.Infrastructure/ \
    --startup-project src/Chirp.Web/
```

Now you can update the database and run the application:
```bash
dotnet ef database update --project src/Chirp.Web
dotnet run --launch-profile Localhost --project src/Chirp.Web
```

**[Note: If you change the database structure, you might want to remove the database container, and follow the steps from step five again]**


## How to run test suite locally
You should be able to run all tests except the E2E-tests following the guide above. This section is specifying how to run our E2E-tests.
    
We are using Selenium Grid to automate our testing. This makes headless end-to-end testing really easy across platforms. Ensure that you have `docker` and run:
```bash
sudo docker run --net="host" -d -p 4444:4444 \
    -v /dev/shm:/dev/shm selenium/standalone-chrome
```
**[Note: This might not work on non-UNIX compliant systems]**

Ensure that the application is running by doing:
```bash
dotnet run --launch-profile Localhost
```
Now we can run 
```bash
dotnet test
```
It is possible to utilize Virtual Network Computing (VNC) to see the tests running. These will by default be running on `http://localhost:7900/` using noVNC with the default password `secret`. 
