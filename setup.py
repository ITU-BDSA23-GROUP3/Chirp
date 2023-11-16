import platform
import json
f = open(".env", "w")
if (platform.platform() == "Windows"):
    f.write("USERSECRETSPATH=$APPDATA/Microsoft/UserSecrets\n")
else:
    f.write("USERSECRETSPATH=$HOME/.microsoft/usersecrets\n")
f.write("USER_SECRETS_ID=c82fceda-b27c-4d6c-ad3e-302f7d63b4d1\n")
f.flush()
with open('./src/Chirp.Web/appsettings.Development.json', 'r+', encoding='utf-8') as f:
    data = json.load(f)
    data["ConnectionStrings"]["DefaultConnection"] = "Server=localhost,1433;Database=Master;User Id=SA;Password=Adsa2023;TrustServerCertificate=True;"

