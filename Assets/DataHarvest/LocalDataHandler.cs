using UnityEngine;
using System.Text;
using System.IO;
using System;

public sealed class LocalDataHandler {
    public bool SaveLocally(string data) {
        string pathname = Application.persistentDataPath + Path.DirectorySeparatorChar + GenerateFileName();
        try {
            File.WriteAllText(pathname, data, Encoding.UTF8);
            LogHelper.Info(this, "data written in " + pathname);
        } catch (Exception ex) {
            LogHelper.Error(this, "while writing to file: " + ex.StackTrace);
            return false;
        }
        return true;
    }

    private string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("{0}-{1}-{2}T{3}:{4}:{5}_{6}", 
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }

}
