using UnityEngine;
using System.IO;

[System.Serializable]
internal class AppConfig {
    public static AppConfig FromFile(string confFile) {
        string txt = File.ReadAllText(confFile);
        return JsonUtility.FromJson<AppConfig>(txt);
    }

    private AppConfig() {}

    public DataServer dataserver;

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
