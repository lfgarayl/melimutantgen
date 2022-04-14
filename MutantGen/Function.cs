using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using MutantGen.BusinessLogic;
using MutantGen.Config;
using MutantGen.Data.Models;
using MutantGen.Data.Repositories;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MutantGen
{
    public class Function
    {
        /// <summary>
        /// Funcion que analiza, almacena y reporta si una secuencia de ADN es Gen Mutante
        /// </summary>
        /// <param name="request">Request de tipo Api Gateway Request para completar la integracion con lambda</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log($"Request Body:{request.Body}");

                //Revisar que el body no sea vacio y se pueda transformar
                if (request.Body != null && request.Body.GetType() != typeof(DnaParameter))
                {
                    var dna = JsonConvert.DeserializeObject<DnaParameter>(request.Body);

                    LambdaLogger.Log($"Iniciando Validaciones del DNA");
                    Validaciones validaciones = new Validaciones();
                    var dnaValido = validaciones.EsDnaValido(dna.Dna);
                    var esGenMutante = false;
                    if (dnaValido)
                    {
                        LambdaLogger.Log($"DNA Valido.");
                        LambdaLogger.Log($"Iniciando El procesamiento de las secuencias de DNA");
                        ProcesamientoDna procesamiento = new ProcesamientoDna();
                        esGenMutante = await procesamiento.RevisarGenMutante(dna.Dna);
                        LambdaLogger.Log($"Es Gen Mutante: {esGenMutante}");
                    }
                    var responseCode = esGenMutante ? (int)HttpStatusCode.OK : (int)HttpStatusCode.Forbidden;

                    var dnaGuardado = new AdnParam()
                    {
                        Dna = dna.Dna,
                        EsMutante = esGenMutante
                    };

                    LambdaLogger.Log($"Iniciando almacenamiento de Gen en BD");

                    var repository = new GenRepository();
                    GenRepository.CreateClient();
                    await repository.GuardarAdn(dnaGuardado);

                    LambdaLogger.Log($"Almacenado Correctamente");

                    return new APIGatewayProxyResponse
                    {
                        StatusCode = responseCode,
                        Body = "El proceso termino exitosamente"
                    };
                }
                else
                {
                    LambdaLogger.Log($"Error Interno en el request del API Gateway");
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Body = "Error Interno en el request del API Gateway"
                    };
                }
            }
            catch(ArgumentException aex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Body = $"{aex.Message} / {aex.ParamName}"
                };
            }
            catch (Exception ex)
            {               
                return new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Body = $"Internal Error. Reason: {ex.Message}"
                };
            }
        }  


        
    }
}
