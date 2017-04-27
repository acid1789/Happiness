using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;


namespace PatchLib
{
    public static class Hash
    {
        const int FILE_CHUNK_SIZE = 1024 * 1024 * 50;

        public static string BytesToString(byte[] hash)
        {
            string str = "";
            foreach (byte b in hash)
                str += b.ToString("X2");
            return str;
        }

        public static string HashFile(string fileName)
        {
            MD5 md5 = MD5.Create();
            
            try
            {
                string hashString = "";
                using (FileStream inputFile = File.OpenRead(fileName))
                {
                    long size = inputFile.Length;
                    while (size > 0)
                    {
                        // Process in 50mb chunks
                        int sizeToRead = (int)Math.Min(size, FILE_CHUNK_SIZE);

                        // Read the file data
                        byte[] fileData = new byte[sizeToRead];
                        int readBytes = inputFile.Read(fileData, 0, sizeToRead);
                        size -= readBytes;

                        // Add to the hash
                        if (inputFile.Length < FILE_CHUNK_SIZE)
                            hashString = BytesToString(md5.ComputeHash(fileData));
                        else
                        {
                            md5.TransformBlock(fileData, 0, fileData.Length, fileData, 0);
                            if (readBytes < FILE_CHUNK_SIZE)
                            {
                                md5.TransformFinalBlock(fileData, 0, 0);
                                hashString = BytesToString(md5.Hash);
                            }
                        }

                    }
                }
                return hashString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
            
        }
    }
}
