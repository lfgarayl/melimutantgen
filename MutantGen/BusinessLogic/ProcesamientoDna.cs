using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace MutantGen.BusinessLogic
{
    public class ProcesamientoDna
    {
        /// <summary>
        /// Analiza el gen de ADN recibido, lo transforma y define si es mutante o no
        /// </summary>
        /// <param name="dna"></param>
        /// <returns>true: Mutante, False: humano</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> RevisarGenMutante(string[] dna)
        {
            try
            {
                var secuenciaMutante = 0;
                var listaIndexRevisados = new List<string>();
                short cantidadSecuencias = 0;
                for (int i = 0; i < dna.Length; i++)
                {
                    char[] letras = dna[i].ToCharArray();
                    for (int j = 0; j < letras.Length; j++)
                    {
                        if (!listaIndexRevisados.Contains($"{i}{j}"))
                        {
                            var lista = await ObtenerSecuencias(i, j, dna);
                            cantidadSecuencias += lista.Item2;
                            if (lista.Item1.Count > 0)
                            {
                                listaIndexRevisados.AddRange(lista.Item1);
                                secuenciaMutante += cantidadSecuencias;
                            }
                            if (secuenciaMutante > 1)
                            {
                                LambdaLogger.Log($"Se ha encontrado mas de una secuencia de letras seguidas");
                                return true;
                            }
                        }
                    }
                }
                LambdaLogger.Log($"No se ha encontrado mas de una secuencia mutante");
                return false;
            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error en el analisis de las secuencias", e);
            }
        }

        /// <summary>
        /// Analiza si hay secuencia vertical sobre el item de la lista actual
        /// </summary>
        /// <param name="indexSecuencia"></param>
        /// <param name="indexLetra"></param>
        /// <param name="dna"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Tuple<List<string>, short>> HaySecuenciaVertical(int indexSecuencia, int indexLetra, string[] dna)
        {
            try
            {
                var listaIndexValidos = new List<string>();
                var listaTemp = new List<string>();
                short cantidadRepetidas = 0;

                int conteo = 1;
                if (dna.Length - indexSecuencia >= 4)
                {
                    for (int i = 0; i < dna.Length; i++)
                    {
                        var indexActual = indexSecuencia;
                        indexSecuencia++;
                        if (!listaIndexValidos.Contains($"{indexActual}{indexLetra}"))
                        {
                            if (listaTemp.Count == 0)
                                listaTemp.Add($"{indexActual}{indexLetra}");

                            if (indexSecuencia >= dna.Length)
                                break;

                            var letraRevision = dna[indexSecuencia].ToCharArray()[indexLetra];
                            if (dna[indexActual].ToCharArray()[indexLetra] == letraRevision)
                            {
                                conteo++;
                                listaTemp.Add($"{indexSecuencia}{indexLetra}");
                                if (conteo == 4)
                                {
                                    listaIndexValidos.AddRange(listaTemp);
                                    cantidadRepetidas++;
                                    listaTemp.Clear();
                                    conteo = 1;
                                }
                            }
                            else
                            {
                                listaTemp.Clear();
                                conteo = 1;
                            }
                        }
                    }
                }

                return new Tuple<List<string>, short>(listaIndexValidos, cantidadRepetidas);
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error en el procesamiento vertical", ex);
            }
        }

        /// <summary>
        /// Analiza si hay secuencia horizontal sobre el item de la lista actual
        /// </summary>
        /// <param name="indexSecuencia"></param>
        /// <param name="indexLetra"></param>
        /// <param name="dna"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Tuple<List<string>, short>> HaySecuenciaHorizontal(int indexSecuencia, int indexLetra, string secuencia)
        {
            try
            {
                var listaIndexValidos = new List<string>();
                var listaTemp = new List<string>();
                short cantidadRepetidas = 0;

                int conteo = 1;
                if (secuencia.Length - indexLetra >= 4)
                {
                    for (int i = 0; i < secuencia.Length; i++)
                    {
                        var indexActual = indexLetra;
                        indexLetra++;
                        if (!listaIndexValidos.Contains($"{indexActual}{indexLetra}"))
                        {

                            if (listaTemp.Count == 0)
                                listaTemp.Add($"{indexSecuencia}{indexActual}");

                            if (indexLetra >= secuencia.Length)
                                break;

                            var letraRevision = secuencia[indexLetra];
                            if (secuencia[indexActual] == letraRevision)
                            {
                                conteo++;
                                listaTemp.Add($"{indexSecuencia}{indexLetra}");
                                if (conteo == 4)
                                {
                                    listaIndexValidos.AddRange(listaTemp);
                                    cantidadRepetidas++;
                                    listaTemp.Clear();
                                    conteo = 1;
                                }
                            }
                            else
                            {
                                listaTemp.Clear();
                                conteo = 1;
                            }
                        }
                    }
                }

                return new Tuple<List<string>, short>(listaIndexValidos, cantidadRepetidas);

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error en el procesamiento horizontal", ex);
            }
        }

        /// <summary>
        /// Analiza si hay secuencia oblicua inclinada en la derecha sobre el item de la lista actual
        /// </summary>
        /// <param name="indexSecuencia"></param>
        /// <param name="indexLetra"></param>
        /// <param name="dna"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Tuple<List<string>, short>> HaySecuenciaOblicuaDerecha(int indexSecuencia, int indexLetra, string[] dna)
        {
            try
            {


                var listaIndexValidos = new List<string>();
                var listaTemp = new List<string>();
                short cantidadRepetidas = 0;

                int conteo = 1;
                if (dna.Length - indexSecuencia >= 4 || dna[indexSecuencia].Length - indexLetra >= 4)
                {
                    for (int i = 0; i < dna.Length; i++)
                    {
                        var indexSecuenciaActual = indexSecuencia;
                        var indexLetraActual = indexLetra;
                        indexSecuencia++;
                        indexLetra++;
                        if (!listaIndexValidos.Contains($"{indexSecuenciaActual}{indexLetraActual}"))
                        {
                            if (listaTemp.Count == 0)
                                listaTemp.Add($"{indexSecuenciaActual}{indexLetraActual}");

                            if (indexSecuencia >= dna.Length || indexLetra >= dna[indexSecuencia].Length)
                                break;

                            var letraRevision = dna[indexSecuencia].ToCharArray()[indexLetra];
                            if (dna[indexSecuenciaActual].ToCharArray()[indexLetraActual] == letraRevision)
                            {
                                conteo++;
                                listaTemp.Add($"{indexSecuencia}{indexLetra}");
                                if (conteo == 4)
                                {
                                    listaIndexValidos.AddRange(listaTemp);
                                    cantidadRepetidas++;
                                    listaTemp.Clear();
                                    conteo = 1;
                                }
                            }
                            else
                            {
                                listaTemp.Clear();
                                conteo = 1;
                            }
                        }
                    }
                }

                return new Tuple<List<string>, short>(listaIndexValidos, cantidadRepetidas);

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error en el procesamiento vertical", ex);
            }
        }

        /// <summary>
        /// Analiza si hay secuencia oblicua inclinada sobre la izquierda sobre el item de la lista actual
        /// </summary>
        /// <param name="indexSecuencia"></param>
        /// <param name="indexLetra"></param>
        /// <param name="dna"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Tuple<List<string>, short>> HaySecuenciaOblicuaIzquierda(int indexSecuencia, int indexLetra, string[] dna)
        {

            try
            {

                var listaIndexValidos = new List<string>();
                var listaTemp = new List<string>();
                short cantidadRepetidas = 0;

                int conteo = 1;
                if (indexSecuencia <= 2 || indexLetra >= 3)
                {
                    for (int i = 0; i < dna.Length; i++)
                    {
                        var indexSecuenciaActual = indexSecuencia;
                        var indexLetraActual = indexLetra;
                        indexSecuencia++;
                        indexLetra--;
                        if (!listaIndexValidos.Contains($"{indexSecuenciaActual}{indexLetraActual}"))
                        {
                            if (listaTemp.Count == 0)
                                listaTemp.Add($"{indexSecuenciaActual}{indexLetraActual}");

                            if (indexSecuencia >= dna.Length || indexLetra < 0)
                                break;

                            var letraRevision = dna[indexSecuencia].ToCharArray()[indexLetra];
                            if (dna[indexSecuenciaActual].ToCharArray()[indexLetraActual] == letraRevision)
                            {
                                conteo++;
                                listaTemp.Add($"{indexSecuencia}{indexLetra}");
                                if (conteo == 4)
                                {
                                    listaIndexValidos.AddRange(listaTemp);
                                    cantidadRepetidas++;
                                    listaTemp.Clear();
                                    conteo = 1;
                                }
                            }
                            else
                            {
                                listaTemp.Clear();
                                conteo = 1;
                            }
                        }
                    }
                }

                return new Tuple<List<string>, short>(listaIndexValidos, cantidadRepetidas);

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error en el procesamiento vertical", ex);
            }
        }

        /// <summary>
        /// Analiza en orden si existe mas de una secuencia mutante seguida, si es asi retorna solo cuando hay mas de una secuencia seguida
        /// </summary>
        /// <param name="indexSecuencia"></param>
        /// <param name="indexLetra"></param>
        /// <param name="dna"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public async Task<Tuple<List<string>, short>> ObtenerSecuencias(int indexSecuencia, int indexLetra, string[] dna)
        {
            try
            {
                var listaSecuencias = new List<string>();
                short cantidadSecuencias = 0;

                var vertical = await HaySecuenciaVertical(indexSecuencia, indexLetra, dna);
                listaSecuencias.AddRange(vertical.Item1);
                cantidadSecuencias = vertical.Item2;

                if (cantidadSecuencias > 1)
                    return new Tuple<List<string>, short>(listaSecuencias, cantidadSecuencias);

                var horizontal = await HaySecuenciaHorizontal(indexSecuencia, indexLetra, dna[indexSecuencia]);
                listaSecuencias.AddRange(horizontal.Item1);
                cantidadSecuencias += horizontal.Item2;

                if (cantidadSecuencias > 1)
                    return new Tuple<List<string>, short>(listaSecuencias, cantidadSecuencias);

                var oblicuaDerecha = await HaySecuenciaOblicuaDerecha(indexSecuencia, indexLetra, dna);
                listaSecuencias.AddRange(oblicuaDerecha.Item1);
                cantidadSecuencias += oblicuaDerecha.Item2;

                if (cantidadSecuencias > 1)
                    return new Tuple<List<string>, short>(listaSecuencias, cantidadSecuencias);

                var oblicuaIzquierda = await HaySecuenciaOblicuaIzquierda(indexSecuencia, indexLetra, dna);
                listaSecuencias.AddRange(oblicuaIzquierda.Item1);
                cantidadSecuencias += oblicuaIzquierda.Item2;

                return new Tuple<List<string>, short>(listaSecuencias, cantidadSecuencias);

            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error en el procesamiento de secuencias", ex);
            }
        }
    }
}
