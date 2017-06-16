using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace Happiness_Android
{
    class FileManager_Android : Happiness.FileManager
    {
        string _happinessPath;

        public FileManager_Android()
        {
            _instance = this;            
            _happinessPath = Activity1.Instance.GetDir("Happiness", FileCreationMode.Private).AbsolutePath;
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