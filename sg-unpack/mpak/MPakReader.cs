using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SgUnpack
{
    public class MPakReader
    {
        private const int DirectoryEntrySize = 256;

        private readonly List<MPakReaderStep> _steps = new List<MPakReaderStep>
        {
            new MPakReaderStep("check MPAK header")
            {
                Len = _ => 8,
                Process = (ctx, bs) => ctx.MPackHeader = Encoding.UTF8.GetString(bs).Sanitise(),
                Success = ctx => ctx.MPackHeader.StartsWith("MPK")
            },
            new MPakReaderStep("get entry count")
            {
                Len = _ => 8,
                Process = (ctx, bs) => ctx.EntryCount = BitConverter.ToInt32(bs, 0),
                Success = ctx => ctx.EntryCount > 0
            },
            new MPakReaderStep("seek to entries") {Len = _ => 48},
            new MPakReaderStep("read entries")
            {
                Len = ctx => ctx.EntryCount * DirectoryEntrySize,
                Process = (ctx, bs) =>
                {
                    var entries =
                        bs.Buffer(DirectoryEntrySize)
                            .Select(entryBytes =>
                            {
                                var nfo = entryBytes.Take(32).Buffer(8).Select(b => BitConverter.ToInt32(b,0)).ToList();
                                var fn = Encoding.UTF8.GetString(entryBytes.Skip(32).ToArray()).Sanitise().Replace("\\", "_");

                                if (String.IsNullOrWhiteSpace(fn))
                                    fn = $"!!{Guid.NewGuid()}".Replace("-","");

                                return new MPakEntry
                                {
                                    FileName = fn,
                                    Offset = nfo[1],
                                    Length = nfo[2]
                                };
                            })
                            .OrderBy(e => e.Offset)
                            .ToList();

                    ctx.Entries = entries;
                },
                Success = ctx => ctx.Entries.Count == ctx.EntryCount
            },
        };

        public MPakData Process(string f)
        {
            var name = Path.GetFileNameWithoutExtension(f);
            using (var ms = new FileStream(f, FileMode.Open, FileAccess.Read))
            {
                var context = new MPakData(f);
                foreach (var step in _steps)
                {
                    var len = step.Len(context);

                    var bs = new byte[len];
                    ms.Read(bs, 0, len);
                    step.Process?.Invoke(context, bs);

                    if (!step.Success?.Invoke(context) ?? false)
                        throw new Exception($"An error occurred while attempting to {step.Name} of '{f}'");
                }

                return context;
            }
        }

        public void Extract(MPakEntry entry, string path, Stream ms)
        {
            ms.Seek(entry.Offset, SeekOrigin.Begin);
            using (var os = new FileStream(path, FileMode.Create, FileAccess.Write))
                ms.CopyBytesTo(os, entry.Length);
        }
    }
}
