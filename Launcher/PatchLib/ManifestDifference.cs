using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchLib
{
    public class ManifestDifference
    {
        public bool Delete;
        public string File;
        public string WebFile;
        public string RemoteHash;
        public WebRequest Request;
        public Zip Decompress;

        public ManifestDifference(string file, string remoteHash)
        {
            File = file;
            RemoteHash = remoteHash;
            Delete = remoteHash == null;
        }
    }
}
