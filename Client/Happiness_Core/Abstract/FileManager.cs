using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Happiness
{
    public abstract class FileManager
    {
        protected static FileManager _instance;
        public static FileManager Instance { get { return _instance; } }

        public abstract string HappinessPath { get; }

        public abstract bool Exists(string filePath);
        public abstract void Delete(string filePath);

        public abstract void WriteAllBytes(string path, byte[] bytes);
        public abstract void WriteAllLines(string path, string[] lines);

        public abstract byte[] ReadAllBytes(string path);
        public abstract string[] ReadAllLines(string path);

        public abstract void CreateDirectory(string path);
    }
}
