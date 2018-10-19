using System.IO;
using UnityEngine;

namespace AWSSDK.Examples
{
    public class Utils : MonoBehaviour
    {
        private static byte[] GetBytesFile(Stream responseStream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public static void ConvertObjectS3ForFile(Stream responseStream, string pathOutputFile = "C:/", string fileNameWithExtesion = "file.txt")
        {
            var bytesFile = GetBytesFile(responseStream);
            File.WriteAllBytes(string.Format("{0}{1}", pathOutputFile, fileNameWithExtesion), bytesFile);
        }

        public static void ConvertObjectS3ForFile(byte[] bytesFile, string pathOutputFile = "C:/", string fileNameWithExtesion = "file.txt")
        {
            File.WriteAllBytes(string.Format("{0}{1}", pathOutputFile, fileNameWithExtesion), bytesFile);
        }
    }
}