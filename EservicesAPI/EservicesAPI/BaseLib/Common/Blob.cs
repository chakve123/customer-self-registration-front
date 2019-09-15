using System.IO;

namespace BaseLib.Common
{
    public static class Blob
    {
        public static byte[] StreamToByteArray(Stream inputStream)
        {
            inputStream.Position = 0;
            BinaryReader reader = new BinaryReader(inputStream);

            byte[] data = reader.ReadBytes((int)inputStream.Length);

            reader.Close();
            inputStream.Close();

            return data;
        }
    }
}