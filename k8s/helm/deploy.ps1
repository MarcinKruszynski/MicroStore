Param(    
    [parameter(Mandatory=$false)][string]$dockerUser,
    [parameter(Mandatory=$false)][string]$dockerPassword,
    [parameter(Mandatory=$false)][string]$externalDns,
    [parameter(Mandatory=$false)][string]$appName="microstore",
    [parameter(Mandatory=$false)][bool]$deployInfrastructure=$true,
    [parameter(Mandatory=$false)][bool]$clean=$true,    
    [parameter(Mandatory=$false)][string]$imageTag="latest"
)

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

$infras = ("identitydb") #, "productdb", "bookingdb", "paymentdb", "notificationdb", "notificationnosqldb", "rabbitmq", "elasticsearch", "kibana")
$charts = ("identityservice", "apigateway", "bookingagg", "productservice", "bookingservice", "paymentservice", "notificationservice", "webapp", "webstatus")

if ($deployInfrastructure) {
    foreach ($infra in $infras) {
        Write-Host "Installing infrastructure: $infra" -ForegroundColor Green
        #helm install $infra     
    }
}

Write-Host "helm charts installed." -ForegroundColor Green




