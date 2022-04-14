**Mutant Gen API **

Programa realizado en .Net Core Lambda
Se divide en dos Lambdas:
1. MutantGen: Valida los registros de ADN, los analiza y almacena en una BD en la nube
2. MutantGenStats: Retorna informacion sobre los analisis de ADNs de la BD e informa cuantos ADNs mutantes encontro, cuantos humanos y el Ratio de ambos.

**Pasos para ejecutar los programas MutantGen y MutantGenStats en Visual Studio localmente **
1. Instalar Visual Studio (VS) Community
2. Instalar minimo la version 3.1 de .Net Core
3. Instalar dependencia para probar en local por medio del AWS .NET Core 3.1 Mock Test Tool: Ejecutar 'dotnet tool install -g Amazon.Lambda.TestTool-2.1'
4. Abrir la solucion .sln por medio de Visual Studio (VS)
5. Dar click derecho al proyecto de VS que se desea ejecutar (MutantGen o MutantGenStats) y seleccionar la opcion "Set as Startup project"
![image](https://user-images.githubusercontent.com/103683478/163465721-80d04eac-b172-45c6-8ec6-d4a4730790b2.png)
6. Ejecutar en el VS usando Mock Lambda Test Tool:
![image](https://user-images.githubusercontent.com/103683478/163464281-448b333f-cb31-458c-aeca-e60387559109.png)
7. Una vez adentro usar este tipo de input:

{
  "body": "{\"dna\": [\"AAACTG\",\"CCTGAG\",\"CCTATG\",\"ATAGCG\",\"CACTTT\",\"AAGGAT\"]}",
  "resource": "/{proxy+}",
  "path": "/path/to/resource",
  "httpMethod": "POST",
  "isBase64Encoded": true,
  "queryStringParameters": {
    "foo": "bar"
  },
  "multiValueQueryStringParameters": {
    "foo": [
      "bar"
    ]
  },
  "pathParameters": {
    "proxy": "/path/to/resource"
  },
  "stageVariables": {
    "baz": "qux"
  },
  "headers": {
    "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
    "Accept-Encoding": "gzip, deflate, sdch",
    "Accept-Language": "en-US,en;q=0.8",
    "Cache-Control": "max-age=0",
    "CloudFront-Forwarded-Proto": "https",
    "CloudFront-Is-Desktop-Viewer": "true",
    "CloudFront-Is-Mobile-Viewer": "false",
    "CloudFront-Is-SmartTV-Viewer": "false",
    "CloudFront-Is-Tablet-Viewer": "false",
    "CloudFront-Viewer-Country": "US",
    "Host": "1234567890.execute-api.us-east-1.amazonaws.com",
    "Upgrade-Insecure-Requests": "1",
    "User-Agent": "Custom User Agent String",
    "Via": "1.1 08f323deadbeefa7af34d5feb414ce27.cloudfront.net (CloudFront)",
    "X-Amz-Cf-Id": "cDehVQoZnx43VYQb9j2-nvCh-9z396Uhbp027Y2JvkCPNLmGJHqlaA==",
    "X-Forwarded-For": "127.0.0.1, 127.0.0.2",
    "X-Forwarded-Port": "443",
    "X-Forwarded-Proto": "https"
  },
  "multiValueHeaders": {
    "Accept": [
      "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
    ],
    "Accept-Encoding": [
      "gzip, deflate, sdch"
    ],
    "Accept-Language": [
      "en-US,en;q=0.8"
    ],
    "Cache-Control": [
      "max-age=0"
    ],
    "CloudFront-Forwarded-Proto": [
      "https"
    ],
    "CloudFront-Is-Desktop-Viewer": [
      "true"
    ],
    "CloudFront-Is-Mobile-Viewer": [
      "false"
    ],
    "CloudFront-Is-SmartTV-Viewer": [
      "false"
    ],
    "CloudFront-Is-Tablet-Viewer": [
      "false"
    ],
    "CloudFront-Viewer-Country": [
      "US"
    ],
    "Host": [
      "0123456789.execute-api.us-east-1.amazonaws.com"
    ],
    "Upgrade-Insecure-Requests": [
      "1"
    ],
    "User-Agent": [
      "Custom User Agent String"
    ],
    "Via": [
      "1.1 08f323deadbeefa7af34d5feb414ce27.cloudfront.net (CloudFront)"
    ],
    "X-Amz-Cf-Id": [
      "cDehVQoZnx43VYQb9j2-nvCh-9z396Uhbp027Y2JvkCPNLmGJHqlaA=="
    ],
    "X-Forwarded-For": [
      "127.0.0.1, 127.0.0.2"
    ],
    "X-Forwarded-Port": [
      "443"
    ],
    "X-Forwarded-Proto": [
      "https"
    ]
  },
  "requestContext": {
    "accountId": "123456789012",
    "resourceId": "123456",
    "stage": "prod",
    "requestId": "c6af9ac6-7b61-11e6-9a41-93e8deadbeef",
    "requestTime": "09/Apr/2015:12:34:56 +0000",
    "requestTimeEpoch": 1428582896000,
    "identity": {
      "cognitoIdentityPoolId": null,
      "accountId": null,
      "cognitoIdentityId": null,
      "caller": null,
      "accessKey": null,
      "sourceIp": "127.0.0.1",
      "cognitoAuthenticationType": null,
      "cognitoAuthenticationProvider": null,
      "userArn": null,
      "userAgent": "Custom User Agent String",
      "user": null
    },
    "path": "/prod/path/to/resource",
    "resourcePath": "/{proxy+}",
    "httpMethod": "POST",
    "apiId": "1234567890",
    "protocol": "HTTP/1.1"
  }
}

Donde el "body" va a contener la informacion de dna que se quiere probar y el listado de strings con las secuencias de ADN.
**NOTA: El Request es de tipo prueba, por lo que lo unico que realmente debe hacer enfasis es en el parametro body que se tiene**

8. Dar click al boton "Execute Function" y justo debajo en la zona de Response: se encuentra la respuesta a este proceso en conjunto con sus logs
![image](https://user-images.githubusercontent.com/103683478/163465433-0d2bb31c-97fc-49e3-8fc6-611070d3f896.png)
**

**Para Ejecutar las pruebas unitarias automaticas**
1. Una vez ubicado en Visual Studio y con la solucion abierta ir a "Test Explorer", existen 20 pruebas unitarias con un code coverage de mas del 90% del codigo, Dar click en ejecutar todas las pruebas:
![image](https://user-images.githubusercontent.com/103683478/163481922-f426aa15-c123-42b2-add0-cd1f15e88f09.png)
2. Una vez se ejecutan todas las pruebas se puede evidenciar si resultan exitosas o no al final del proceso.

**Para probar API REST**

Se puede probar la API Rest por medio de Postman usando las siguientes URLs:

curl --location --request POST 'https://y7j09fz8vh.execute-api.us-east-1.amazonaws.com/release/mutant' \
--header 'Content-Type: application/json' \
--data-raw '{
    "dna": [
        "AAACTG",
        "CCTGAG",
        "CCTATG",
        "ATAGCG",
        "CTCTTT",
        "AAGGAT"
    ]
}'


curl --location --request GET 'https://y7j09fz8vh.execute-api.us-east-1.amazonaws.com/release/stats'

