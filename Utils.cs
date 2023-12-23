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

        public static byte[] GetByteArrayFromHexString(string hexString) {
            if(hexString.Length % 2 != 0) {
                throw new ArgumentException("Hex string length must be even.");
            }

            byte[] byteArray = new byte[hexString.Length / 2];

            for(int i = 0; i < hexString.Length; i += 2) {
                string byteString = hexString.Substring(i, 2);
                byteArray[i / 2] = byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber);
            }

            return byteArray;
        }
    }
}
