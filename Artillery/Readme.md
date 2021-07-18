# Artillery - Load testing

This is some experimentation using Artillery tool.

## Install dependencies

```
npm install -g artillery@latest
```

## Check the installation

```
artillery dino
```

## Run the test

Run test depending of your environment
```
artillery run -e debug .\load-test.yml
artillery run -e local .\load-test.yml
artillery run -e prod .\load-test.yml
```

Export report
```
artillery run -e debug .\load-test.yml -o Report.json
artillery run -e local .\load-test.yml -o Report.json
artillery run -e prod .\load-test.yml -o Report.json
```

You can see the json report visualy using this web site : https://reportviewer.artillery.io/

Build from this github repository : https://github.com/artilleryio/report-viewer

