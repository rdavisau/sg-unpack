using System.Collections.Generic;
using System.IO;

namespace SgUnpack
{
    public class MPakData
    {
        public MPakData(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileNameWithoutExtension(fileName);
        }

        public string FileName { get; set; }
        public string Name { get; set; }
        public string MPackHeader { get; set; }
        public int EntryCount { get; set; }
        public List<MPakEntry> Entries { get; set; }
    }
}
