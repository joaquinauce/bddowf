using System;
using System.Linq;

namespace WebAPIAFA.Helpers
{
    public class RandomStringGenerator
    {
        char[] ValueAfanumeric = { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'X', 'C', 'V', 'B', 'N', 'M' };
        char[] ValueNumeric = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        /// <summary>
        /// Funcion que retorna una clave alfanumerica del tamaño especificado por parametro
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetRandom(int size)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[size];
            Random random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            string finalString = new String(stringChars);

            return finalString;
        }

        public string GenerarPass()
        {
            Random ram = new Random();

            string Password = String.Empty;

            int lengthPass = -1;

            //Next(minvalue, maxvalue - 1)
            lengthPass = ram.Next(4, 9);

            while (Password.Length < 4)
            {
                string _valor = ValueNumeric[ram.Next(0, 9)].ToString();

                Password += _valor;
            }

            //Acá obtenemos los caracteres en mayúscula para la generación del password.
            while (Password.Length < 9)
            {
                string _valor = ValueAfanumeric[ram.Next(0, 25)].ToString();

                if (!Password.ToUpper().Contains(_valor))
                {
                    Password += _valor;
                }
            }


            //Acá obtenemos los valores numéricos para la generación del password.
            while (Password.Length < 7)
            {
                string _valor = ValueNumeric[ram.Next(0, 9)].ToString();

                if (!Password.Contains(_valor))
                {
                    Password += _valor;
                }
            }

            return Password;
        }
    }
}
