# Home.ClinicalPortal

My implementation of some HL7v3 soap services, educational purpose.

### Framework used

- dotnet core 3.1
- SoapCore
- message-builder-dotnet
- Azure API for FHIR
- Hl7.Fhir.R4

## Setup the environment

To setup the environment, beside the folder of this repo, you should also clone two others repos.

```
git clone https://github.com/microsoft/fhir-server-samples.git
git clone https://github.com/microsoft/fhir-server-samples
```

Follow the steps describe here : https://github.com/microsoft/fhir-server-samples#prerequisites

Docker command to generate test data for the solution :

```
docker run --rm -v ${PWD}/output:/output --name synthea-docker intersystemsdc/irisdemo-base-synthea:version-1.3.4 -p 25
```

You can see a demo on youtube : https://www.youtube.com/watch?v=5vS7Iq9vpXE

## Once the environment is deployed

There is two services actually in the solution. The Registry and Laboratory, before starting the services, go set the appropriate environment variables to connect to the Fhir API.

The app will read the environment variable called ```FHIR_API_URL``` as the url of the Fhir api. You can authenticate the app by copying the bearer token in the about page of the dashboard (https://[baseAppName]dash.azurewebsites.net) and past the value in the ```FHIR_BEARER_TOKEN``` environment variable. Then run the app.

### Use the client app

To interact with the soap services. The client app is in Home.ClinicalPortal.Client folder.

### Implementation reference

Registry : 

https://nasjonalikt.no/Documents/Prosjekter/Avsluttede%20prosjekter/Tiltak%2033.2%20HL7%20Implementation%20Guide.pdf