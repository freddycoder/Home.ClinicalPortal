
$app = az ad app create --display-name BlazorOnFhir-Debug --reply-urls https://localhost:5001/signin-oidc

Write-Output $app

$app = $app | ConvertFrom-Json

$update = az ad app update --id $app.appId --set logoutUrl=https://localhost:5001/signout-oidc

Write-Output $update