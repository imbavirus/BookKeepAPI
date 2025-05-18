# setup-database.ps1

# Define project paths relative to the script location (solution root)
$applicationProjectPath = ".\Application" # Or "BookKeepAPI.Application"
$apiProjectPath = ".\API"                 # Or "BookKeepAPI.API"

dotnet ef database update --project $applicationProjectPath --startup-project $apiProjectPath
