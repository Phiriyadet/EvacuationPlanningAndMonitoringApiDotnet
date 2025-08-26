
# Evacuation Planning and Monitoring API

ระบบวางแผนการอพอพและติดตามการอพยพ


## Tech Stack

**API:** C# .net API

**Database:** MSSQL

**Cache:** Redis




## Tools

- Visual studio 2022
- Docker Desktop
- Postman
## Environment Variables

* secrets.json

`"Jwt:Key": "KEY_FOR_JWT"`

`"ConnectionStrings:RedisConnection": "YOUR_REDIS_CONNECTION"`

`"ConnectionStrings:DatabaseConnection": "YOUR_MSSQL_CONNECTION"`

`"Admin:Password": "YOUR_ADMIN_PASSWORD"`


## Connection Strings
* appsetings.json
```
"ConnectionStrings": {
  "DatabaseConnection": "",
  "RedisConnection": ""
},
"Jwt": {
  "Key": "",
  "Issuer": "EvacuationApi",
  "Audience": "EvacuationApiUsers"
},
"Admin": {
  "Username": "admin",
  "Password": "" 
}
```
## Nuget Package
* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.Tools
* Microsoft.EntityFrameworkCore.SqlServer
* StackExchange.Redis;
* Microsoft.IdentityModel.Tokens;


## Commands
### use in src/
* add migrations
`dotnet ef migrations add InitialCreate --output-dir Data/Migrations --project ./Evacuation.Infrastructure --startup-project ./Evacuation.API`
* update database
`dotnet ef database update --project ./Evacuation.Infrastructure --startup-project ./Evacuation.API`
