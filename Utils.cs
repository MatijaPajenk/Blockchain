using System.Security.Cryptography;
using System.Text;

namespace Blockchain {
    static internal class Utils {
        public static byte[] ComputeHash(byte[] data) {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(data);
            return hashBytes;
        }

        public static string GetHexString(byte[] data) {
            var builder = new StringBuilder();
            for(int i = 0; i < data.Length; i++) {
                builder.Append(data[i].ToString("x2"));
            }
            return builder.ToString();
        }


    }
}
