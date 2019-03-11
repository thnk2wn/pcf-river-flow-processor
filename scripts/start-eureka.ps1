$eurekaLocalPath = "C:\Projects\eureka"

# JAVA_HOME must be set for maven build
# needs to be set to a JDK dir not JRE otherwise:
# No compiler is provided in this environment. Perhaps you are running on a JRE rather than a JDK?
$env:JAVA_HOME = "C:\Program Files\Java\jdk1.8.0_191"

if (!(Test-Path $eurekaLocalPath)) {
    git clone https://github.com/spring-cloud-samples/eureka.git $eurekaLocalPath
}

Push-Location $eurekaLocalPath
# This will take some time on the initial run
.\mvnw spring-boot:run
Pop-Location