using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Happiness_Desktop
{
    class FileManager_Desktop : Happiness.FileManager
    {
        string _happinessPath;

        public FileManager_Desktop()
        {
            _instance = this;

            string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _happinessPath = localData + "/Happiness/";
        }

        public override string HappinessPath { get { return _happinessPath; } }

        public override void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public override void Delete(string filePath)
        {
            File.Delete(filePath);
        }

        public override bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public override byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public override string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public override void WriteAllBytes(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public override void WriteAllLines(string path, string[] lines)
        {
            File.WriteAllLines(path, lines);
        }
    }
}
