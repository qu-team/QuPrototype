using UnityEngine;
using System.IO;
using System.Diagnostics.CodeAnalysis;

[System.Serializable]
internal class AppConfig {
    public static AppConfig FromFile(string confFile) {
        string txt = File.ReadAllText(confFile);
        return JsonUtility.FromJson<AppConfig>(txt);
    }

    private AppConfig() {}

#pragma warning disable 0649
    public DataServer dataserver;
    public bool debug;
#pragma warning restore

    override public string ToString() {
        return JsonUtility.ToJson(this);
    }

    [System.Serializable]
    public struct DataServer {
        public string address;
        public int port;
        public string path;
    }
}
