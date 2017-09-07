using UnityEngine;
using System.IO;

// Mapping of appconfig.json
[System.Serializable]
public class AppConfig {
    public static AppConfig FromFile(string confFile) {
        string txt = File.ReadAllText(confFile);
        return JsonUtility.FromJson<AppConfig>(txt);
    }

    public static AppConfig FromResources(string fname) {
        return JsonUtility.FromJson<AppConfig>(Resources.Load<TextAsset>(fname).text);
    }

    private AppConfig() {}

    override public string ToString() {
        return JsonUtility.ToJson(this);
    }

    public string GetRequestURL() {
        return forceLocalhost
            ? "http://localhost:8000/"
            : dataserver.address + ":" + dataserver.port + dataserver.path;
    }

    [System.Serializable]
    public struct DataServer {
        public string address;
        public int port;
        public string path;
    }

#pragma warning disable 0649
    public DataServer dataserver;
    public bool debug;
    public bool forceLocalhost;
#pragma warning restore
}
