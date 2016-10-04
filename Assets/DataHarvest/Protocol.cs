using System;
using UnityEngine;

[System.Serializable]
public struct IcarusMsg {
    public IcAppdata appdata;
    public bool debug;
    public string userdata;
    public IcTimestamp timestamp;
}

[System.Serializable]
public struct IcAppdata {
    public string uuid;
    public bool voiceover;
    public string device;
    public string lang;
    public string appname;
    public uint appversion;
}

[System.Serializable]
public struct IcTimestamp {
    // ISO 8601 date yyyy-MM-ddTHH:mm:ss.fff+TZ
    public string utc;
    public string user;
}

public static class Protocol {
    // Adds the JSON fields required by the Icarus server protocol to userdata `data`.
    public static string WrapUserData(string data) {
        IcarusMsg msg = new IcarusMsg();
        msg.debug = true;
        msg.appdata.uuid = "NOT_IMPLEMENTED";
        msg.appdata.voiceover = false;
        msg.appdata.device = SystemInfo.deviceModel;
        msg.appdata.appname = "Qu";
        msg.appdata.appversion = 1;
        msg.timestamp.utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff+zzz");
        msg.timestamp.utc = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff+zzz");
        msg.userdata = data;

        return UnityEngine.JsonUtility.ToJson(msg);
    }
}
