using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Net;
using MutantGen.Config;
using MutantGenStats.ResponseModels;
using MutantGen.BusinessLogic;
using MutantGen.Data.Repositories;
using MutantGen.Data.Models;

namespace MutantGen.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void DnaIsMutant200OkTest()
        {
            var function = new MutantGen.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] dna = new string[6];
            dna[0] = "AAACTG";
            dna[1] = "CCTGAG";
            dna[2] = "CCTAGG";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            DnaParameter parameter = new DnaParameter()
            {
                Dna = dna
            };

            request.Body = JsonSerializer.Serialize(parameter);

            var response = function.FunctionHandler(request, context);

            Assert.Equal((int)HttpStatusCode.OK, response.Result.StatusCode);
        }

        [Fact]
        public void DnaIsNotMutantForbiddenTest()
        {
            var function = new MutantGen.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] dna = new string[6];
            dna[0] = "AAACTG";
            dna[1] = "CCTGAG";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            DnaParameter parameter = new DnaParameter()
            {
                Dna = dna
            };

            request.Body = JsonSerializer.Serialize(parameter);

            var response = function.FunctionHandler(request, context);

            Assert.Equal((int)HttpStatusCode.Forbidden, response.Result.StatusCode);
        }

        [Fact]
        public void InvalidDnaWrongLettersHTTPTest()
        {
            var function = new MutantGen.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] dna = new string[6];
            dna[0] = "AAACTR";
            dna[1] = "CCTGAA";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAR";

            DnaParameter parameter = new DnaParameter()
            {
                Dna = dna
            };

            request.Body = JsonSerializer.Serialize(parameter);

            var response = function.FunctionHandler(request, context);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.Result.StatusCode);
        }

        [Fact]
        public void InvalidDnaWrongLettersTest()
        {
            var function = new MutantGen.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] dna = new string[6];
            dna[0] = "AAACTR";
            dna[1] = "CCTGAA";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAR";

            DnaParameter parameter = new DnaParameter()
            {
                Dna = dna
            };

            request.Body = JsonSerializer.Serialize(parameter);

            var response = function.FunctionHandler(request, context);
            Assert.Contains(ErrorDefinitions.ERROR_LETRAS_INVALIDAS, response.Result.Body);
        }

        [Fact]
        public void InvalidDnaWrongDimensionHTTPTest()
        {
            var function = new MutantGen.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] dna = new string[6];
            dna[0] = "AAACTRTT";
            dna[1] = "CCTGAA";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAR";

            DnaParameter parameter = new DnaParameter()
            {
                Dna = dna
            };

            request.Body = JsonSerializer.Serialize(parameter);

            var response = function.FunctionHandler(request, context);

            Assert.Equal((int)HttpStatusCode.BadRequest, response.Result.StatusCode);
        }

        [Fact]
        public void InvalidDnaWrongDimensionTest()
        {
            var function = new MutantGen.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] dna = new string[6];
            dna[0] = "AAACTATT";
            dna[1] = "CCTGAA";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAR";

            DnaParameter parameter = new DnaParameter()
            {
                Dna = dna
            };

            request.Body = JsonSerializer.Serialize(parameter);

            var response = function.FunctionHandler(request, context);

            Assert.Contains(ErrorDefinitions.ERROR_DIMENSION_INVALIDA, response.Result.Body);
        }

        [Fact]
        public void SaveAndGetMutantDnaTest()
        {
            var functionSave = new MutantGen.Function();
            var functionScan = new MutantGenStats.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] mutantDna = new string[6];
            mutantDna[0] = "AAAATG";
            mutantDna[1] = "CCTGAG";
            mutantDna[2] = "CGTATT";
            mutantDna[3] = "ATTTCG";
            mutantDna[4] = "CATTAT";
            mutantDna[5] = "AATGAT";

            DnaParameter parameterMutant = new DnaParameter() { Dna = mutantDna };

            request.Body = JsonSerializer.Serialize(parameterMutant);
            var responseMutant = functionSave.FunctionHandler(request, context);

            var responseDB = functionScan.FunctionHandler(context);

            var response = JsonSerializer.Deserialize<Stats>(responseDB.Result.Body);

            Assert.True(response.Count_mutant_dna > 0);
        }

        [Fact]
        public void SaveAndGetHumanDnaTest()
        {
            var functionSave = new MutantGen.Function();
            var functionScan = new MutantGenStats.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] humanDna = new string[6];
            humanDna[0] = "AAAATG";
            humanDna[1] = "CCTGAG";
            humanDna[2] = "CGTATT";
            humanDna[3] = "ATTTCG";
            humanDna[4] = "CATTAT";
            humanDna[5] = "AATGAT";

            DnaParameter parameterHuman = new DnaParameter() { Dna = humanDna };

            request.Body = JsonSerializer.Serialize(parameterHuman);
            var responseHuman = functionSave.FunctionHandler(request, context);

            var responseDB = functionScan.FunctionHandler(context);

            var response = JsonSerializer.Deserialize<Stats>(responseDB.Result.Body);

            Assert.True(response.Count_human_dna > 0);
        }

        [Fact]
        public void SaveAndGetHumanAndMutantDnaTest()
        {
            var functionSave = new MutantGen.Function();
            var functionScan = new MutantGenStats.Function();
            var context = new TestLambdaContext();
            APIGatewayProxyRequest request = new APIGatewayProxyRequest();

            string[] mutantDna = new string[6];
            mutantDna[0] = "AAAATG";
            mutantDna[1] = "CCTGAG";
            mutantDna[2] = "CGTATT";
            mutantDna[3] = "ATTTCG";
            mutantDna[4] = "CATTAT";
            mutantDna[5] = "AATGAT";

            string[] humanDna = new string[6];
            humanDna[0] = "AAAATG";
            humanDna[1] = "CCTGAG";
            humanDna[2] = "CGTATT";
            humanDna[3] = "ATTTCG";
            humanDna[4] = "CATTAT";
            humanDna[5] = "AATGAT";

            DnaParameter parameterMutant = new DnaParameter() { Dna = mutantDna };
            DnaParameter parameterHuman = new DnaParameter() { Dna = humanDna };

            request.Body = JsonSerializer.Serialize(parameterMutant);
            var responseMutant = functionSave.FunctionHandler(request, context);

            request.Body = JsonSerializer.Serialize(parameterHuman);
            var responseHuman = functionSave.FunctionHandler(request, context);

            var responseDB = functionScan.FunctionHandler(context);

            var response = JsonSerializer.Deserialize<Stats>(responseDB.Result.Body);

            Assert.True(response.Count_mutant_dna > 0 && response.Count_human_dna > 0);
        }

        [Fact]
        public void ValidDnaLettersValidationTest()
        {
            var validaciones = new Validaciones();

            string[] dna = new string[6];
            dna[0] = "AAACTT";
            dna[1] = "CCTGAA";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            var response = validaciones.EsDnaValido(dna);
            Assert.True(response);
        }

        [Fact]
        public void ValidDnaDimensionValidationTest()
        {
            var validaciones = new Validaciones();

            string[] dna = new string[6];
            dna[0] = "AAACTA";
            dna[1] = "CCTGAA";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAA";

            var response = validaciones.EsDnaValido(dna);

            Assert.True(response);
        }

        [Fact]
        public void DnaIsMutantTest()
        {
            var procesamiento = new ProcesamientoDna();

            string[] dna = new string[6];
            dna[0] = "AAACTG";
            dna[1] = "CCTGAG";
            dna[2] = "CCTAGG";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            var response = procesamiento.RevisarGenMutante(dna);

            Assert.True(response.Result);
        }

        [Fact]
        public void DnaIsNotMutantTest()
        {
            var procesamiento = new ProcesamientoDna();

            string[] dna = new string[6];
            dna[0] = "AAACTG";
            dna[1] = "CCTGAG";
            dna[2] = "CCTAGC";
            dna[3] = "ATATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            var response = procesamiento.RevisarGenMutante(dna);

            Assert.False(response.Result);
        }

        [Fact]
        public void DnaThereIsVerticalTest()
        {
            var procesamiento = new ProcesamientoDna();

            string[] dna = new string[6];
            dna[0] = "CAACTG";
            dna[1] = "CCTGAG";
            dna[2] = "CCTAGG";
            dna[3] = "CTATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            var response = procesamiento.HaySecuenciaVertical(0,0,dna);

            Assert.True(response.Result.Item2 > 0);
        }

        [Fact]
        public void DnaThereIsHorizontalTest()
        {
            var procesamiento = new ProcesamientoDna();

            string[] dna = new string[6];
            dna[0] = "CCCCTG";
            dna[1] = "ACTGAG";
            dna[2] = "TCTAGG";
            dna[3] = "CTATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            var response = procesamiento.HaySecuenciaHorizontal(0, 0, "CCCCTG");

            Assert.True(response.Result.Item2 > 0);
        }

        [Fact]
        public void DnaThereIsDiagonalLeftTest()
        {
            var procesamiento = new ProcesamientoDna();

            string[] dna = new string[6];
            dna[0] = "CCCCTA";
            dna[1] = "ACTGAG";
            dna[2] = "TCTAGG";
            dna[3] = "CTATCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            var response = procesamiento.HaySecuenciaOblicuaIzquierda(0, 5, dna);

            Assert.True(response.Result.Item2 > 0);
        }

        [Fact]
        public void DnaThereIsDiagonalRightTest()
        {
            var procesamiento = new ProcesamientoDna();

            string[] dna = new string[6];
            dna[0] = "CCCCTA";
            dna[1] = "ACTGAG";
            dna[2] = "TCCTGG";
            dna[3] = "CTACCG";
            dna[4] = "CACTAT";
            dna[5] = "AAGGAT";

            var response = procesamiento.HaySecuenciaOblicuaDerecha(0, 0, dna);

            Assert.True(response.Result.Item2 > 0);
        }

        [Fact]
        public void DnaThereAreSecuencesTest()
        {
            var procesamiento = new ProcesamientoDna();

            string[] dna = new string[6];
            dna[0] = "CCCCTA";
            dna[1] = "ACTGAG";
            dna[2] = "TCCTGG";
            dna[3] = "CTACCG";
            dna[4] = "CACTCT";
            dna[5] = "AAGGAT";

            var response = procesamiento.ObtenerSecuencias(0, 0, dna);

            Assert.True(response.Result.Item2 > 0);
        }

        [Fact]
        public void DnaSaveTest()
        {
            var repository = new GenRepository();

            string[] dna = new string[6];
            dna[0] = "CCCCTA";
            dna[1] = "ACTGAG";
            dna[2] = "TCCTGG";
            dna[3] = "CTACCG";
            dna[4] = "CACTCT";
            dna[5] = "AAGGAT";

            AdnParam param = new AdnParam()
            {
                Dna = dna,
                EsMutante = true
            };

            var response = repository.GuardarAdn(param);

            Assert.True(true);
        }

        [Fact]
        public void DnaScanTest()
        {
            var repository = new GenRepository();

            var response = repository.ObtenerTodoAdn();

            Assert.True(response.Result.Count>0);
        }
    }
}
