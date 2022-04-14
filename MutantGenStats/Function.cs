using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MutantGen.Data.Repositories;
using MutantGenStats.ResponseModels;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MutantGenStats
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log("Iniciando Analisis de todos los Genes en BD");
                var repository = new GenRepository();
                LambdaLogger.Log("Creando Cliente DynamoDB");
                GenRepository.CreateClient();
                LambdaLogger.Log("Consultando Adn s");
                var result = await repository.ObtenerTodoAdn();

                LambdaLogger.Log("Validando humanos y mutantes");
                decimal humans = result.Where(x => !x.EsMutante).Count();
                decimal mutants = result.Where(x => x.EsMutante).Count();
                decimal ratioStat = humans == 0 ? 0 : mutants / humans;

                var stats = new Stats()
                {
                    Count_human_dna = humans,
                    Count_mutant_dna = mutants,
                    Ratio = Convert.ToDecimal(string.Format("{0:0.0}", ratioStat))
                };

                LambdaLogger.Log("Finalizado proceso exitosamente");

                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Body = JsonSerializer.Serialize(stats)
                };

            }
            catch (Exception ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = ex.Message
                };
            }

            
        }
    }
}
