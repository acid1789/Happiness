using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace PatchLib
{
    public class Zip
    {
        Thread _decompressThread;
        string _compressedFile;
        string _outputFile;
        float _progress;
        bool _finished;

        public Zip()
        {
        }
        
        public void DecompressAsync(string compressedFile, string outputFile)
        {
            _finished = false;
            _compressedFile = compressedFile;
            _outputFile = outputFile;
            _decompressThread = new Thread(new ThreadStart(DecompressThreadFunc) );
            _decompressThread.Name = "Decompress: " + outputFile;
            _decompressThread.Start();
        }

        void DecompressThreadFunc()
        {
            byte[] data = new byte[1024 * 1024 * 25];
            long totalBytesProcessed = 0;
            using (FileStream fs = File.OpenRead(_compressedFile))
            {
                float estimatedFileSize = fs.Length * 4;
                using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
                {
                    using (FileStream output = File.Create(_outputFile))
                    {
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = ds.Read(data, 0, data.Length);
                            output.Write(data, 0, bytesRead);

                            totalBytesProcessed += bytesRead;
                            _progress = Math.Min(totalBytesProcessed / estimatedFileSize, 1);
                        } while( bytesRead >= data.Length );
                    }
                }
            }
            _progress = 1;
            _finished = true;
        }

        public float Progress { get { return _progress; } }
        public bool Finished { get { return _finished; } }


        public static byte[] Decompress(byte[] compressed)
        {
            byte[] data = null;
            using (DeflateStream input = new DeflateStream(new MemoryStream(compressed), CompressionMode.Decompress))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    input.CopyTo(output);
                    data = output.ToArray();
                }
            }

            return data;
        }

        public static void DecompressFile(string compressedFile, string outputFile)
        {
            using (FileStream fs = File.OpenRead(compressedFile))
            {
                using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
                {
                    using (FileStream output = File.Create(outputFile))
                    {
                        ds.CopyTo(output);
                    }
                }
            }
        }

        public static void CompressFile(string inputFile, string outputFile)
        {
            using (FileStream fs = File.OpenRead(inputFile))
            {
                using (FileStream output = File.Create(outputFile))
                {
                    using (DeflateStream ds = new DeflateStream(output, CompressionLevel.Optimal))
                    {
                        fs.CopyTo(ds);
                    }
                }
            }
        }
    }
}
