using UnityEngine;
using System.Text;
using System.IO;
using System.IO.Compression;
using System;

public sealed class LocalDataHandler {
    string persistentDataPath;

    public LocalDataHandler(string path) {
        persistentDataPath = path;
    }

    // Saves the string `data` to a local file with an unique name and gzip's it.
    public bool SaveLocally(string data) {
        string pathname = persistentDataPath + Path.DirectorySeparatorChar + GenerateFileName();
        try {
            File.WriteAllText(pathname, data + "\r\n", Encoding.UTF8);
            LogHelper.Info(this, "data written in " + pathname);
        } catch (Exception ex) {
            LogHelper.Error(this, "while writing to file: " + ex.StackTrace);
            return false;
        }

        try {
            // Zip the file
            using (var compressedStream = File.Create(pathname + ".gz")) {
                using (var stream = File.Open(pathname, FileMode.Open, FileAccess.Read)) {
                    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress)) {
                        byte[] buffer = new byte[4096];
                        int nRead;
                        while ((nRead = stream.Read(buffer, 0, buffer.Length)) != 0) {
                            zipStream.Write(buffer, 0, nRead);
                        }
                    }
                }
            }
            LogHelper.Info(this, "file gzipped to " + pathname + ".gz");
            // Delete the uncompressed file
            File.Delete(pathname);
        } catch (Exception ex) {
            LogHelper.Warn(this, "while compressing file: " + ex.StackTrace);
            // Not a fatal error: consider the 'SaveLocally' process a success
        }
        return true;
    }

    private string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("{0}-{1}-{2}T{3}:{4}:{5}_{6}", 
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }

}
