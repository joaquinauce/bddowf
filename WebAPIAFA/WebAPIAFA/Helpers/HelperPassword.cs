using System.Security.Cryptography;
using System.Text;

namespace WebAPIAFA.Helpers
{
    public class HelperPassword
    {
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] salt)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool ValidatePassword(string password, byte[] passwordHash, byte[] salt)
        {
            using (HMACSHA512 hmac = new HMACSHA512(salt))
            {
                byte[] passwordEnHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < passwordEnHash.Length; i++)
                {
                    if (passwordEnHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }
    }
}
