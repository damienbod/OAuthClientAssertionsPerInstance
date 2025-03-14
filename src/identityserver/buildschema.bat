rmdir /S /Q "Data/Migrations"

dotnet ef migrations add Users -c ApplicationDbContext -o Data/Migrations

dotnet ef migrations add Users -c ConfigurationDbContext -o Data/Migrations
