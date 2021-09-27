#AppConfig Key Value
$SecretKey = 'platform:SecretValue'
$SecretValue = 'This is a secret'

#KeyVault secret name
$SecretName = 'SecretInKeyvault'

### Add the keyvault value and return the Id to the secret in KeyVault
$vaultId = (az keyvault secret set --vault-name $keyvaultname --name $SecretName --value $SecretValue | ConvertFrom-Json).id

## Add the keyvault reference to the appconfig
az appconfig kv set-keyvault -n $appconfigname --key $SecretKey --secret-identifier $vaultId --yes
