using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class Extensions
{
    public static string Sanitise(this string s)
        => s.Replace("\0", "");

    public static IEnumerable<byte[]> Buffer(this IEnumerable<byte> bytes, int len)
        => bytes.Select((b, i) => Tuple.Create(b, i))
            .GroupBy(t => t.Item2 / len, t => t.Item1)
            .Select(bs => bs.ToArray());

    public static void CopyBytesTo(this Stream input, Stream output, int bytes)
    {
        var buffer = new byte[32768];
        var read = 0;
        while (bytes > 0 &&
               (read = input.Read(buffer, 0, Math.Min(buffer.Length, bytes))) > 0)
        {
            output.Write(buffer, 0, read);
            bytes -= read;
        }
    }

    public static void CreateDirectoryIfNotExists(string outputDir)
    {
        if (!Directory.Exists(outputDir))
        {
            try
            {
                Directory.CreateDirectory(outputDir);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Could not create output directory '{outputDir}'. Aborting");
                throw ex;
            }
        }
    }

    public static string[] GetInputs(string inputPath, string inputMask)
    {
        if (File.Exists(inputPath))
            return new[] { inputPath };
        else if (Directory.Exists(inputPath))
            return Directory.GetFiles(inputPath, inputMask, SearchOption.TopDirectoryOnly);

        return null;
    }
}
