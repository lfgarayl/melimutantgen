using System;
using System.Text.RegularExpressions;
using MutantGen.Config;

namespace MutantGen.BusinessLogic
{
    public class Validaciones
    {
        public  bool EsDnaValido(string[] dna)
        {
            string pattern = @"\b[ATCG]+\b(?![,])";
            Regex rg = new Regex(pattern);

            foreach (var item in dna)
            {
                if (!rg.IsMatch(item))
                    throw new ArgumentException("El mensaje no contiene las letras validas A,C,T,G, verifique de nuevo y reintente", ErrorDefinitions.ERROR_LETRAS_INVALIDAS);

                if (!(item.Length == dna.Length))
                    throw new ArgumentException("Las dimensiones del dna no son validas", ErrorDefinitions.ERROR_DIMENSION_INVALIDA);
            }
            return true;
        }
    }
}
    