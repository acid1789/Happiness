using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading;

namespace PatchGen
{
    class Program
    {
        const int FILE_CHUNK_SIZE = 1024 * 1024 * 50;

        static string s_inputPath;
        static string s_outputPath;

        class ThreadData
        {
            public Thread thread;
            public string manifestEntry;
        }

        static void FileProcessThread(object outobj)
        {
            ThreadData output = (ThreadData)outobj;
            string file = Thread.CurrentThread.Name;

            output.manifestEntry = ProcessFile(file);
        }

        static string ProcessFile(string file)
        {
            MD5 md5 = MD5.Create();

            string fileName = file.Substring(s_inputPath.Length);
            try
            {
                // Setup output file
                string outputFile = s_outputPath + fileName;
                string outputDir = Path.GetDirectoryName(outputFile);
                Directory.CreateDirectory(outputDir);
                FileStream outputStream = File.Create(outputFile);
                DeflateStream deflate = new DeflateStream(outputStream, CompressionLevel.Optimal);

                FileStream inputFile = File.OpenRead(file);
                long size = inputFile.Length;
                string hashString = "";
                while (size > 0)
                {
                    // Process in 50mb chunks
                    int sizeToRead = (int)Math.Min(size, FILE_CHUNK_SIZE);

                    // Read the file data
                    byte[] fileData = new byte[sizeToRead];
                    int readBytes = inputFile.Read(fileData, 0, sizeToRead);
                    size -= readBytes;

                    // Add to the zip stream
                    deflate.Write(fileData, 0, fileData.Length);

                    // Add to the hash
                    if (inputFile.Length < FILE_CHUNK_SIZE)
                        hashString = PatchLib.Hash.BytesToString(md5.ComputeHash(fileData));
                    else
                    {
                        md5.TransformBlock(fileData, 0, fileData.Length, fileData, 0);
                        if (readBytes < FILE_CHUNK_SIZE)
                        {
                            md5.TransformFinalBlock(fileData, 0, 0);
                            hashString = PatchLib.Hash.BytesToString(md5.Hash);
                        }
                    }

                }
                deflate.Close();
                outputStream.Close();

                // Add the hash to the manifest
                return fileName + "," + hashString;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        static void Main(string[] args)
        {
            bool multiThreaded = true;

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: PatchGen [distribution folder] [output folder]");
                return;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("  Distribution Folder: {0} does not exist", args[0]);
                return;
            }

            try
            {
                if (Directory.Exists(args[1]))
                    Directory.Delete(args[1], true);
                Directory.CreateDirectory(args[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to setup output folder\n" + ex.ToString());
                return;
            }

            DateTime start = DateTime.Now;

            bool error = false;
            s_inputPath = args[0];
            s_outputPath = args[1];
            List <string> hashManifest = new List<string>();
            List <ThreadData> threads = new List<ThreadData>();
            string[] files = Directory.GetFiles(args[0], "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (multiThreaded)
                {
                    ThreadData td = new ThreadData();
                    td.thread = new Thread(new ParameterizedThreadStart(FileProcessThread));
                    td.thread.Name = file;
                    td.thread.Start(td);
                    
                    threads.Add(td);
                }
                else
                {
                    Console.WriteLine("Processing file: " + file);
                    string manifestEntry = ProcessFile(file);
                    if (manifestEntry == null)
                        error = true;
                    else
                        hashManifest.Add(manifestEntry);
                }
            }

            if (multiThreaded)
            {
                foreach (ThreadData td in threads)
                {
                    Console.WriteLine("Waiting for file: " + td.thread.Name);
                    td.thread.Join();

                    if (td.manifestEntry == null)
                        error = true;
                    else
                        hashManifest.Add(td.manifestEntry);
                }
            }

            if (error)
                Console.WriteLine("Not generating manifest due to errors");
            else
            {
                // Generate the manifest file
                try
                {
                    File.WriteAllLines(args[1] + "/manifest", hashManifest.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to write manifest file\n"  + ex.ToString());
                }
            }
            TimeSpan s = DateTime.Now - start;
            Console.WriteLine("TotalTime: {0} seconds", s.TotalSeconds);
            System.Diagnostics.Debug.WriteLine("TotalTime: {0} seconds", s.TotalSeconds);
        }
    }
}
