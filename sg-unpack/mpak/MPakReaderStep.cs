using System;

namespace SgUnpack
{
    public class MPakReaderStep
    {
        public MPakReaderStep(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public Func<MPakData, int> Len { get; set; }
        public Action<MPakData, byte[]> Process { get; set; }
        public Func<MPakData, bool> Success { get; set; }
    }
}
