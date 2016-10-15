using UnityEngine;
using System.Text;
using System.IO;
using System.IO.Compression;
using System;

internal sealed class LocalDataHandler {
    // Don't try to uncompress files larger than this (uncompressed size). 
    const int MAX_BUF_SIZE = 1024 * 1024 * 50; // 50 KB

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

    public string LoadCompressed(string fname) {
        // Check uncompressed size
        uint ucsize = UncompressedSize(fname);
        if (ucsize > MAX_BUF_SIZE) {
            LogHelper.Warn(this, "file " + fname + " is greater than " + MAX_BUF_SIZE + ": deleting.");
            File.Delete(fname);
            return null;
        }
        byte[] buf = new byte[ucsize];
        using (var compressedStream = File.Open(fname, FileMode.Open)) {
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                int n = zipStream.Read(buf, 0, buf.Length);
                if (n != buf.Length) {
                    LogHelper.Error(this, "failed to read data from " + fname);
                    return null;
                }
                return Encoding.UTF8.GetString(buf);
            }
        }
    }

    string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("{0}-{1}-{2}T{3}:{4}:{5}_{6}", 
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }

    uint UncompressedSize(string fname) {
        using (var fs = File.OpenRead(fname)) {
            fs.Position = fs.Length - 4;
            var b = new byte[4];
            fs.Read(b, 0, 4);
            uint length = BitConverter.ToUInt32(b, 0);
            return length;
        }
    }
}
