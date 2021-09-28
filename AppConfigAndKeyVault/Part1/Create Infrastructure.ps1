az login
################# Dev ################

$AppConfigName = "MyAppconfigUnique001"
$KeyVaultName = "MyKeyVaultUnique001"
$ResourceGroup = 'MyResourceGroup'
$Location = 'UK South'

#AppConfig Key Value
$NonSecretKey=  'platform:NonSecretValue'
$NonSecretValue = 'This is not a secret'

az keyvault purge -n $KeyVaultName

# Create the infrastructure needed
az group create -n $ResourceGroup -l $Location
az keyvault create -g $ResourceGroup -n $KeyVaultName
az appconfig create -g $ResourceGroup -n $AppConfigName -l $Location

#Add the current user to the key vault access policy to get and list
$currentUserObjectId = az ad signed-in-user show --query objectId -o tsv
az keyvault set-policy --name $keyVaultName --key-permissions get list import --secret-permissions get list --object-id $currentUserObjectId

#Remove any exisiting App Config values. 
az appconfig kv delete --name $AppConfigName --key * --label * --yes

## Set a non secret value in the appconfig
az appconfig kv set -n $appconfigname --key $NonSecretKey --value $NonSecretValue --yes    

#Add the connection string to an Envirinment variable so we can get it out in our C# application
Write-Host "Copy this value into the C# App"
az appconfig credential list -g $ResourceGroup -n $appConfigName --query "[?name=='Primary'].connectionString" | ConvertFrom-Json
