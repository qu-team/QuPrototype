using UnityEngine;
using System.Text;
using System.IO;
using System.IO.Compression;
using System;

internal sealed class LocalDataHandler {
    string persistentDataPath;

    public LocalDataHandler(string path) {
        persistentDataPath = path;
    }

    // Saves the string `data` to a local file with an unique name and gzip's it.
    public bool SaveCompressed(string data) {
        string pathname = persistentDataPath + Path.DirectorySeparatorChar + GenerateFileName() + ".gz";
        using (var compressedStream = File.Create(pathname)) {
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress)) {
                byte[] buf = Encoding.UTF8.GetBytes(data);
                zipStream.Write(buf, 0, buf.Length);
            }
        }
        LogHelper.Info(this, "data written to " + pathname);
        return true;
    }

    string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("{0}-{1}-{2}T{3}:{4}:{5}_{6}", 
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }
}
