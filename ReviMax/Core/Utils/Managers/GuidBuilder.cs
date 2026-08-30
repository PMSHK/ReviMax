using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.Core.Utils.Managers
{
    internal class GuidBuilder
    {
        private static readonly Guid NameSpaceGuid = Guid.Parse("0256212F-91BE-4759-8168-B285BC05B024");
        public static string CreateGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static Guid CreateVersion5Guid(string str)
        {
            if (string.IsNullOrEmpty(str)) return Guid.Empty;
                byte[] namespaceBytes = NameSpaceGuid.ToByteArray();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(namespaceBytes, 0, 4);
                Array.Reverse(namespaceBytes, 4, 2);
                Array.Reverse(namespaceBytes, 6, 2);
            }

            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            using (SHA1 sha1 = SHA1.Create()) 
            {
                byte[] hash = sha1.ComputeHash(namespaceBytes.Concat(inputBytes).ToArray());
                hash[6] = (byte)((hash[6] & 0x0f) | 0x50);
                hash[8] = (byte)((hash[8] & 0x3f) | 0x80);
                return new Guid(hash.Take(16).ToArray());
            }
            
        }
    }
}
