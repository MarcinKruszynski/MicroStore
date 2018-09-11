Param(
    [parameter(Mandatory=$false)][string]$registry,
    [parameter(Mandatory=$false)][string]$dockerUser,
    [parameter(Mandatory=$false)][string]$dockerPassword,
    [parameter(Mandatory=$false)][string]$imageTag="latest",
    [parameter(Mandatory=$false)][bool]$buildImages=$true,
    [parameter(Mandatory=$false)][bool]$pushImages=$true,
    [parameter(Mandatory=$false)][string]$dockerOrg="marcinkruszynski"
)

$useDockerHub = [string]::IsNullOrEmpty($registry)

if ([string]::IsNullOrEmpty($imageTag)) {
    Write-Host "Image tag cannot be empty" -ForegroundColor Red
    exit
}
Write-Host "Docker image Tag: $imageTag" -ForegroundColor Yellow

if ($buildImages) {
    Write-Host "Building Docker images tagged with '$imageTag'" -ForegroundColor Yellow
    $env:TAG=$imageTag
    docker-compose -p .. -f ../docker-compose.yml build
}

if (-not [string]::IsNullOrEmpty($dockerUser)) {
    $registryFDQN =  if (-not $useDockerHub) {$registry} else {"index.docker.io/v1/"}

    Write-Host "Logging in to $registryFDQN as user $dockerUser" -ForegroundColor Yellow
    if ($useDockerHub) {
        docker login -u $dockerUser -p $dockerPassword
    }
    else {
        docker login -u $dockerUser -p $dockerPassword $registryFDQN
    }
    
    if (-not $LastExitCode -eq 0) {
        Write-Host "Login failed" -ForegroundColor Red
        exit
    }
}

if ($pushImages) {
    Write-Host "Pushing images to $registry/$dockerOrg..." -ForegroundColor Yellow
    $services = ("identityservice", "apigateway", "bookingagg", "productservice", "bookingservice", "paymentservice", "notificationservice", "webapp", "webstatus")

    foreach ($service in $services) {
        $imageFqdn = if ($useDockerHub)  {"$dockerOrg/${service}"} else {"$registry/$dockerOrg/${service}"}
        docker tag ${service}:$imageTag ${imageFqdn}:$imageTag
        docker push ${imageFqdn}:$imageTag
    }
}





