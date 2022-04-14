
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Runtime;
using MutantGen.Data.Config;
using MutantGen.Data.Models;

namespace MutantGen.Data.Repositories
{
    public class GenRepository
    {
        private static AmazonDynamoDBClient Client;

        public static bool CreateClient()
        {
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            // This client will access the US East 1 region.
            clientConfig.RegionEndpoint = RegionEndpoint.USEast1;
            SecretManager secretManager = new SecretManager();
            var secrets  = secretManager.Get("arn:aws:secretsmanager:us-east-1:425280054633:secret:mainSecret-nrqrMA");
            var keys = JsonSerializer.Deserialize<Secret>(secrets);
            var awsCredentials = new BasicAWSCredentials(keys.ak, keys.sk);
            Client = new AmazonDynamoDBClient(awsCredentials, clientConfig);
            return true;
        }

        public async Task GuardarAdn(AdnParam dna)
        {
            try
            {
                Table tablaDna = Table.LoadTable(Client, "Adn");
                var dnaDoc = new Document
                {
                    ["Id"] = Guid.NewGuid(),
                    ["Secuencia"] = dna.Dna,
                    ["EsMutante"] = dna.EsMutante
                };

                LambdaLogger.Log("Iniciando Almacenamiento DNA en BD");
                await tablaDna.PutItemAsync(dnaDoc);
                LambdaLogger.Log("Registrado Almacenamiento DNA en BD exitosamente");
            }
            catch (Exception ex)
            {
                LambdaLogger.Log($"Fallo al registrar la entrada en la DB Dynamo: {ex.Message}");
                throw ex;
            }

        }

        public async Task<List<AdnParam>> ObtenerTodoAdn()
        {
            var results = new List<AdnParam>();

            try
            {
                Table tablaDna = Table.LoadTable(Client, "Adn");
                ScanOperationConfig config = new ScanOperationConfig();

                LambdaLogger.Log("Iniciando Consulta DNA en BD");
                Search search = tablaDna.Scan(config);

                List<Document> documentList = new List<Document>();
                do
                {
                    documentList = await search.GetNextSetAsync();
                    foreach (var document in documentList)
                    {
                        AdnParam dnas = new AdnParam()
                        {
                            Dna = new[] { document["Secuencia"].ToString() },
                            EsMutante = Convert.ToBoolean(Convert.ToUInt32(document["EsMutante"]))
                        };
                        results.Add(dnas);
                    }
                } while (!search.IsDone);
                LambdaLogger.Log("Consultas de DNA en BD realizadas exitosamente");
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Fallo al obtener todas las secuencias de ADN");
                throw ex;
            }

            return results;
        }
    }
}
