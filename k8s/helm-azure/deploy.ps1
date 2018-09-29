Param(    
    [parameter(Mandatory=$false)][string]$dockerUser,
    [parameter(Mandatory=$false)][string]$dockerPassword,
    [parameter(Mandatory=$false)][string]$externalDns="localhost",
    [parameter(Mandatory=$false)][string]$appName="microstore",
    [parameter(Mandatory=$false)][bool]$deployInfrastructure=$true,
    [parameter(Mandatory=$false)][bool]$clean=$true,
    [parameter(Mandatory=$false)][string]$aksName="",
    [parameter(Mandatory=$false)][string]$aksRg="",
    [parameter(Mandatory=$false)][string]$imageTag="latest"
)

class InfraRecord {
    [string] $Name
    [string] $Image
}

$dns = $externalDns

if ($externalDns -eq "aks") {
    if  ([string]::IsNullOrEmpty($aksName) -or [string]::IsNullOrEmpty($aksRg)) {
        Write-Host "Error: When using -dns aks, MUST set -aksName and -aksRg too." -ForegroundColor Red
        exit 1
    }
    Write-Host "Getting DNS of AKS of AKS $aksName (in resource group $aksRg)..." -ForegroundColor Green
    $dns = $(az aks show -n $aksName  -g $aksRg --query addonProfiles.httpApplicationRouting.config.HTTPApplicationRoutingZoneName)
    if ([string]::IsNullOrEmpty($dns)) {
        Write-Host "Error getting DNS of AKS $aksName (in resource group $aksRg). Please ensure AKS has httpRouting enabled AND Azure CLI is logged & in version 2.0.37 or higher" -ForegroundColor Red
        exit 1
    }
    $dns = $dns -replace '[\"]'
    Write-Host "DNS base found is $dns. Will use $appName.$dns for the app!" -ForegroundColor Green
    $dns = "$appName.$dns"
}

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

$newRec2 = [InfraRecord] @{ Name = 'productdb'; Image = 'stable/postgresql'}
$infras.Add($newRec2)

$newRec3 = [InfraRecord] @{ Name = 'bookingdb'; Image = 'stable/postgresql'}
$infras.Add($newRec3)

$newRec4 = [InfraRecord] @{ Name = 'paymentdb'; Image = 'stable/postgresql'}
$infras.Add($newRec4)

$newRec5 = [InfraRecord] @{ Name = 'notificationdb'; Image = 'stable/postgresql'}
$infras.Add($newRec5)

$newRec6 = [InfraRecord] @{ Name = 'notificationnosqldb'; Image = 'stable/mongodb'}
$infras.Add($newRec6)

$newRec7 = [InfraRecord] @{ Name = 'rabbitmq'; Image = 'stable/rabbitmq'}
$infras.Add($newRec7)


$charts = ("identityservice", "apigateway", "bookingagg", "productservice", "bookingservice", "paymentservice", "notificationservice", "webapp", "webstatus")

if ($deployInfrastructure) {
    foreach ($infra in $infras) {
        Write-Host "Installing infrastructure: $($infra.Name)" -ForegroundColor Green

        if ([string]::IsNullOrEmpty($($infra.Image))) {
		   Write-Host "helm install --name $($infra.Name) $($infra.Name)" -ForegroundColor Green
           helm install --name $($infra.Name) $($infra.Name)
        }
		else {
		   Write-Host "helm install --name $($infra.Name) -f ./$($infra.Name)/values.yaml $($infra.Image)" -ForegroundColor Green
           helm install --name $($infra.Name) -f ./$($infra.Name)/values.yaml $($infra.Image)
		}
    }

	Start-Sleep -Seconds 120
}

foreach ($chart in $charts) {
    Write-Host "Installing: $chart" -ForegroundColor Green

    helm install --values app.yaml --set inf.k8s.dns=$dns --name="$chart" $chart
}

Write-Host "helm charts installed." -ForegroundColor Green




