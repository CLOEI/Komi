using System.Security.Cryptography;
using System.Text;

namespace Komi.lib.utils
{
    public class Proton
    {
        public static string GenerateKlv(string protocol, string version, string rid)
        {
            string[] salts = {
                "e9fc40ec08f9ea6393f59c65e37f750aacddf68490c4f92d0d2523a5bc02ea63",
                "c85df9056ee603b849a93e1ebab5dd5f66e1fb8b2f4a8caef8d13b9f9e013fa4",
                "3ca373dffbf463bb337e0fd768a2f395b8e417475438916506c721551f32038d",
                "73eff5914c61a20a71ada81a6fc7780700fb1c0285659b4899bc172a24c14fc1"
            };

            string[] constantValues = {
                HashSha256(HashMd5(HashSha256(protocol))),
                HashSha256(HashSha256(version)),
                HashSha256(HashSha256(protocol) + salts[3])
            };

            string result = HashSha256(
                $"{constantValues[0]}{salts[0]}{constantValues[1]}{salts[1]}{HashSha256(HashMd5(HashSha256(rid)))}{salts[2]}{constantValues[2]}"
            );

            return result;
        }

        private static string HashSha256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private static string HashMd5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public static uint HashString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }

            uint acc = 0x55555555;
            foreach (byte b in Encoding.UTF8.GetBytes(input))
            {
                acc = (acc >> 27) + (acc << 5) + b;
            }
            return acc;
        }
    }
}