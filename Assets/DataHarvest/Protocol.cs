using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct IcarusMsg {
    public IcAppdata appdata;
    public bool debug;
    public List<DataBundle> userdata;
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
    public static string WrapUserData(List<DataBundle> userdata) {
        IcarusMsg msg = new IcarusMsg();
        msg.debug = true;
        msg.appdata.uuid = "NOT_IMPLEMENTED";
        msg.appdata.voiceover = false;
        msg.appdata.device = SystemInfo.deviceModel;
        msg.appdata.appname = "Qu";
        msg.appdata.appversion = 1;
        msg.timestamp.utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.timestamp.user = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.userdata = userdata;

        return UnityEngine.JsonUtility.ToJson(msg);
    }
}
