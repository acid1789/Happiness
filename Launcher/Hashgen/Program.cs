using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hashgen
{
    class Program
    {
        static void Main(string[] args)
        {
            PatchLib.PatchLog.Initialize("Hashgen.log");

            List<string> inputFiles = new List<string>();
            string outputFile = null;

            foreach (string arg in args)
            {
                if( arg.ToLower().StartsWith("-out=") )
                    outputFile = arg.Substring(5);
                else
                    inputFiles.Add(arg);
            }
            
            if (inputFiles.Count <= 0 || outputFile == null)
            {
                Console.WriteLine("Usage: Hashgen.exe -out=output_file input_file1 input_file2 ...");
                return;
            }

            PatchLib.Manifest m = new PatchLib.Manifest(null);
            foreach (string inputFile in inputFiles)
            {
                try
                {
                    string hash = PatchLib.Hash.HashFile(inputFile);
                    PatchLib.PatchLog.Print("Hash file {0} -> {1}", inputFile, hash);
                    m.AddEntry(inputFile, hash);
                }
                catch (Exception ex)
                {
                    PatchLib.PatchLog.Print("Failed to hash file: " + inputFile + "\n" + ex.ToString());
                    return;
                }
            }
            m.Save(outputFile);

            PatchLib.PatchLog.Shutdown();
        }
    }
}
