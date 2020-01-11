using System;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace OneSTools.BracketsFileTest
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var connStr = new SqlConnectionStringBuilder()
            {
                DataSource = "localhost",
                InitialCatalog = "struct",
                IntegratedSecurity = true
            }.ToString();

            var connection = new SqlConnection(connStr);

            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT [SerializedData] FROM [struct].[dbo].[DBSchema]";
            var data = (byte[])await cmd.ExecuteScalarAsync();

            //var str = await DecompressDataAsync(data);
            var str = Encoding.UTF8.GetString(data);

            var d = BracketsFile.BracketsFileParser.Parse(str);

            return 0;
        }

        /// <summary>
        /// Unpacks and returns bytes array that was packed with the help of "Deflate" algorithm
        /// </summary>
        /// <param name="data">Source byte array</param>
        /// <returns></returns>
        private static async Task<string> DecompressDataAsync(byte[] data)
        {
            string decompressedData;

            using (var source = new MemoryStream(data))
            using (var destination = new MemoryStream())
            using (var deflateStream = new DeflateStream(source, CompressionMode.Decompress))
            {
                var buffer = new byte[data.Length];
                await deflateStream.CopyToAsync(destination, buffer.Length);
                decompressedData = Encoding.UTF8.GetString(destination.ToArray());
            }

            return decompressedData;
        }
    }
}
