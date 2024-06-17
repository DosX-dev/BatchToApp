using System;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

class GeneratedWithBatchToApp { }

public class Program {
    public static void Main() {
        string cmdTemp = Path.ChangeExtension(Path.GetTempFileName(), ".cmd");

        File.WriteAllBytes(cmdTemp, Decompress(ReadResource("embeddedBatchScript")));

        File.SetAttributes(cmdTemp, FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Temporary);

        ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo(cmdTemp) {
            WorkingDirectory = Environment.CurrentDirectory,
            UseShellExecute = false,
            Arguments = Environment.CommandLine,
            CreateNoWindow = /* {IS_HIDDEN} */,
            RedirectStandardError = true
        };

        using (var process = System.Diagnostics.Process.Start(processInfo)) {
            process.WaitForExit();

            if (process.ExitCode != 0) {
                Environment.ExitCode = process.ExitCode;
            }
        }

        File.Delete(cmdTemp);
    }


    static byte[] ReadResource(string resourceName) {
        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
        using (MemoryStream ms = new MemoryStream()) {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    static byte[] Decompress(byte[] data) {
        using (MemoryStream compressedMs = new MemoryStream(data))
        using (DeflateStream deflateStream = new DeflateStream(compressedMs, CompressionMode.Decompress))
        using (MemoryStream decompressedMs = new MemoryStream()) {
            deflateStream.CopyTo(decompressedMs);
            return decompressedMs.ToArray();
        }
    }
}