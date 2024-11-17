using System.Security.Cryptography;


namespace Citizen_E_Tax_API.Services
{
    public class Helper
    {
        public static string GenerateTxnNumber()
        {
            Random random = new Random();
            string txn = "";
            for (int i = 0; i < 10; i++)
            {
                txn += random.Next(0, 10).ToString();
            }
            return txn;
        }


        public static string GenerateReference()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(10));
        }
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password can't be empty");
            }
            else
            {
                var hashpass = BCrypt.Net.BCrypt.HashPassword(password);
                return hashpass;
            }
        }

        public static bool VerifyHashPasswod(string password, string hashpass)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password can't be empty");
            }
            return BCrypt.Net.BCrypt.Verify(password, hashpass);
        }
    }
}
