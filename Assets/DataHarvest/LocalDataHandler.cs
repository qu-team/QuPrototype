using UnityEngine;
using System.Text;
using System.IO;
#if UNITY_WSA
    // Here we know "for sure" that this lib will work
    using System.IO.Compression;
#else
    // Else, use drop-in replacement from https://github.com/Hitcents/Unity.IO.Compression
    using Unity.IO.Compression;
#endif
using System;

internal sealed class LocalDataHandler {
    // Don't try to uncompress files larger than this (uncompressed size).
    const int MAX_BUF_SIZE = 1024 * 50; // 50 KB

    string persistentDataPath;

    public LocalDataHandler(string path) {
        persistentDataPath = path;
    }

    // Saves the string `data` to a local file and gzip's it.
    // If filename is not give, generate an unique file name with GenerateFileName().
    public bool SaveCompressed(string data, string filename = "") {
        string pathname = persistentDataPath + "/" +
            (filename.Length == 0 ? GenerateFileName() : filename) + ".gz";
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
        if (!fname.EndsWith(".gz"))
            fname += ".gz";
        LogHelper.Debug(this, "Loading file " + fname);
        // Check uncompressed size
        int ucsize = UncompressedSize(fname);
        if (ucsize < 0) {
            LogHelper.Warn(this, "file " + fname + " is too small to be a gzip file: deleting.");
            File.Delete(fname);
            return null;
        } else if (ucsize > MAX_BUF_SIZE) {
            LogHelper.Warn(this, "file " + fname + " is greater than " + MAX_BUF_SIZE + " bytes: deleting.");
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
#if UNITY_WSA
                return Encoding.UTF8.GetString(buf, 0, ucsize);
#else
                return Encoding.UTF8.GetString(buf);
#endif
            }
        }
    }

    /** Tries to delete file `fname`[.gz]. Returns whether a file with that name existed or not. */
    public bool DeleteFile(string fname) {
        if (!fname.EndsWith(".gz"))
            fname += ".gz";
        if (!File.Exists(fname)) {
            LogHelper.Info(this, "tried to remove inexistent file " + fname);
            return false;
        }
        File.Delete(fname);
        LogHelper.Ok(this, "deleted file " + fname);
        return true;
    }

    string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("qudata_{0}-{1}-{2}T{3}:{4}:{5}_{6}",
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }

    int UncompressedSize(string fname) {
        using (var fs = File.OpenRead(fname)) {
            if (fs.Length < 5) return -1;
            fs.Position = fs.Length - 4;
            var b = new byte[4];
            fs.Read(b, 0, 4);
            int length = BitConverter.ToInt32(b, 0);
            return length;
        }
    }
}
