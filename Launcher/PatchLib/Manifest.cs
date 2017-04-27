using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PatchLib
{
    public class Manifest
    {
        Dictionary<string, string> _files;

        public Manifest(string[] entries)
        {
            _files = new Dictionary<string, string>();
            if (entries != null)
            {
                foreach (string entry in entries)
                {
                    string[] pieces = entry.Split(',');
                    if( pieces.Length == 2 )
                        _files[pieces[0]] = pieces[1];
                }
            }
        }

        public string GetHash(string key)
        {
            if( _files.ContainsKey(key) )
                return _files[key];
            return null;
        }

        public static ManifestDifference[] Compare(Manifest local, Manifest remote)
        {
            List<ManifestDifference> differences = new List<ManifestDifference>();

            // Do all the local entries first finding all files that are different and all that need to be deleted
            foreach (KeyValuePair<string, string> entry in local._files)
            {
                string remoteHash = remote.GetHash(entry.Key);
                if (remoteHash != entry.Value)
                {
                    differences.Add(new ManifestDifference(entry.Key, remoteHash));
                }
            }

            // Now go through the remote entries to find any that dont exist locally that need to be downloaded
            foreach (KeyValuePair<string, string> entry in remote._files)
            {
                string localHash = local.GetHash(entry.Key);
                if( localHash == null )
                    differences.Add(new ManifestDifference(entry.Key, entry.Value));
            }

            return differences.ToArray();
        }

        public void AddEntry(string file, string hash)
        {
            _files[file] = hash;
        }

        public string[] GetFileList()
        {
            List<string> list = new List<string>(_files.Keys);
            return list.ToArray();
        }

        public void Save(string outputFile)
        {
            List<string> lines = new List<string>();
            foreach (KeyValuePair<string, string> kvp in _files)
                lines.Add(kvp.Key + "," + kvp.Value);

            try
            {
                File.WriteAllLines(outputFile, lines.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Console.WriteLine(ex.ToString());                
            }
        }

        public static Manifest Load(string manifestFile)
        {
            string[] entries = null;
            try
            {
                entries = File.ReadAllLines(manifestFile);
            }
            catch (Exception){ }

            Manifest m = null;
            if (entries == null)
            {
                string path = Path.GetDirectoryName(manifestFile);                
                m = GenerateManifest(path);
            }
            else
            {
                m = new Manifest(entries);
            }
            
            return m;
        }

        public static Manifest GenerateManifest(string path)
        {
            PatchLog.Print("Generating Manifest: " + path + "/manifest");
            Manifest m = new Manifest(null);

            Directory.CreateDirectory(path);
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string relative = file.Substring(path.Length);
                if( Path.GetFileName(relative) == "manifest" )
                    continue;

                string hash = Hash.HashFile(file);
                m._files[relative] = hash;
            }

            m.Save(path + "/manifest");
            return m;
        }
    }
}
