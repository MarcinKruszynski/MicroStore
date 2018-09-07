Param(    
    [parameter(Mandatory=$false)][string]$dockerUser,
    [parameter(Mandatory=$false)][string]$dockerPassword,
    [parameter(Mandatory=$false)][string]$externalDns,
    [parameter(Mandatory=$false)][string]$appName="microstore",
    [parameter(Mandatory=$false)][bool]$deployInfrastructure=$true,
    [parameter(Mandatory=$false)][bool]$clean=$true,    
    [parameter(Mandatory=$false)][string]$imageTag="latest"
)

class InfraRecord {
    [string] $Name
    [string] $Image
}

$dns = $externalDns

# Initialization & check commands
if ([string]::IsNullOrEmpty($dns)) {
    Write-Host "No DNS specified. Ingress resources will be bound to public ip" -ForegroundColor Yellow
}

if ($clean) {
    Write-Host "Cleaning previous helm releases..." -ForegroundColor Green
    helm delete --purge $(helm ls -q) 
    Write-Host "Previous releases deleted" -ForegroundColor Green
}

Write-Host "Begin MicroStore installation using Helm" -ForegroundColor Green

$infras = [System.Collections.Generic.List[InfraRecord]]::new()

$newRec1 = [InfraRecord] @{ Name = 'identitydb'; Image = 'stable/postgresql'}
$infras.Add($newRec1)

# "productdb", "bookingdb", "paymentdb", "notificationdb", "notificationnosqldb", "rabbitmq", "elasticsearch", "kibana"

$charts = ("identityservice", "apigateway", "bookingagg", "productservice", "bookingservice", "paymentservice", "notificationservice", "webapp", "webstatus")

if ($deployInfrastructure) {
    foreach ($infra in $infras) {
        Write-Host "Installing infrastructure: $($infra.Name)" -ForegroundColor Green
        Write-Host "helm install --name $($infra.Name) -f ./$($infra.Name)/values.yaml $($infra.Image)" -ForegroundColor Green
        helm install --name $($infra.Name) -f ./$($infra.Name)/values.yaml $($infra.Image)
    }
}

Write-Host "helm charts installed." -ForegroundColor Green




