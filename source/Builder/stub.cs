using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

class GeneratedWithBatchToApp { }

public class Program {
    private static string appName = Assembly.GetExecutingAssembly().GetName().Name;
    private static string appId = /* {APP_ID} */;

    public static void Main(string[] args) {
        try {
            RunApplication();
        } catch (Exception ex) {
            MessageBox.Show("Unable to execute.", appName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void RunApplication() {
        string cmdTemp = Path.Combine(Path.GetTempPath(), "b2a." + appName + "." + appId + ".cmd");

        if (File.Exists(cmdTemp)) {
            File.SetAttributes(cmdTemp, FileAttributes.Normal);
            File.Delete(cmdTemp);
        }

        File.WriteAllBytes(cmdTemp, Decompress(ReadResource("embeddedBatchScript")));

        File.SetAttributes(cmdTemp, FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Temporary | /* {FS_PROTECTION} */);

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

        File.SetAttributes(cmdTemp, FileAttributes.Normal);
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