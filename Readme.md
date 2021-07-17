# Home.ClinicalPortal

My implementation of some HL7v3 soap services, educational purpose.

### Framework used

- dotnet core 3.1, .net 5.0
- SoapCore
- message-builder-dotnet
- Azure API for FHIR
- Hl7.Fhir.R4
- Docker

## Setup the environment on your local machine

To setup the environment on your local machine, you will need to download to other repos beside the current repo.

```
git clone https://github.com/microsoft/fhir-server-samples.git
git clone https://github.com/microsoft/health-architectures.git
```

After, you will need to create a TestData folder inside the current repo and run this command inside it :

```
docker run --rm -v ${PWD}/output:/output --name synthea-docker intersystemsdc/irisdemo-base-synthea:version-1.3.4 -p 25
```

Then at the root directory of the current repo, execute 

```
docker-compose up
```

Wait a little, and in the docker desktop app, you will see that the api has stoped. Just restart it, there is a sync problem at the start and the api try to connect to the database too soon.

Then inside the FhirImporter folder, run :

> You can ajuste the paralelism parameter in the FhirImporter/Properties/launchSettings.json file

```
dotnet restore
dotnet run
```

Once it is done, launch the BlazorOnFhir app to look at the data.

```
cd ..\BlazorOnFhir
dotnet restore
dotnet run
```

Watch the demo video : https://youtu.be/SXPkaujXius

## Setup the environment on azure

Duration somewhere between 30 minutes and 2 hours. 
Required an azure subscription and docker installed on your computer.

To setup the environment, beside the folder of this repo, you should also clone two others repos.

You can see a demo on youtube : https://www.youtube.com/watch?v=5vS7Iq9vpXE

```
git clone https://github.com/microsoft/fhir-server-samples.git
git clone https://github.com/microsoft/health-architectures.git
```

Follow the steps describe here : https://github.com/microsoft/fhir-server-samples#prerequisites

Docker command to generate test data for the solution :

```
docker run --rm -v ${PWD}/output:/output --name synthea-docker intersystemsdc/irisdemo-base-synthea:version-1.3.4 -p 25
```

Upload the json into the import folder of the azure storage created by the deployment.

## Once the environment is deployed

There is two services actually in the solution. The Registry and Laboratory, before starting the services, go set the appropriate environment variables to connect to the Fhir API.

The app will read the environment variable called ```FHIR_API_URL``` as the url of the Fhir api. You can authenticate the app by copying the bearer token in the about page of the dashboard (https://[baseAppName]dash.azurewebsites.net) and past the value in the ```FHIR_BEARER_TOKEN``` environment variable. Then run the app.

### Use the client app

To interact with the soap services. The client app is in Home.ClinicalPortal.Client folder.

## Build and run Registry images

```
docker build -t registry:local -f .\Registry\Dockerfile .
docker run -d -p 41558:80 -e ASPNETCORE_ENVIRONMENT=Production -e FHIR_API_URL=http://host.docker.internal:8080 registry:local
```

## Nugetise the proxy librairy

```
cd ..\health-architectures\FHIR\FHIRProxy\FHIRProxy
dotnet restore
dotnet build -c Release
dotnet pack -c Release -o ..\Packages
```

And the package upload packages via the upload packages of nuget.org

### Implementation reference

Registry : 

https://nasjonalikt.no/Documents/Prosjekter/Avsluttede%20prosjekter/Tiltak%2033.2%20HL7%20Implementation%20Guide.pdf
