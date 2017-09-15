using System;
using System.Globalization;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct IcarusMsg<T> {
    public IcAppdata appdata;
    public bool debug;
    public T userdata;
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

[System.Serializable]
public struct IcQuUserdata {
    public Devicedata devicedata;
    public List<DataBundle> gamedata;
}

[System.Serializable]
public struct Devicedata {
    public float screenDPI;
    public int screenHeight;
    public int screenWidth;
}

[System.Serializable]
public struct IcCrash {
    public IcCrashLogData exception;
}

[System.Serializable]
public struct IcCrashLogData {
    public string type;
    public string logString;
    public string stackTrace;
}

/* The data structure is:
 * {
 *    "appdata": IcAppdata
 *    "debug": bool
 *    "userdata": {
 *        "devicedata": Devicedata
 *        "gamedata": List<DataBundle>
 *    }
 *    "timestamp": IcTimestamp
 * }
 */
public static class Protocol {

    public const string UUID_KEY = "IcUUID";

    private const uint APP_VERSION = 2;
    private const string APP_NAME = "qU";

    // Adds the JSON fields required by the Icarus server protocol to `gamedata`.
    public static string WrapUserData<T>(T userdata) {
        IcarusMsg<T> msg = new IcarusMsg<T>();
        msg.debug = true;
        msg.appdata.uuid = GetUUID();
        msg.appdata.voiceover = false;
        msg.appdata.lang = L10N.CurrentLanguage.ToString();
        msg.appdata.device = SystemInfo.deviceModel;
        msg.appdata.appname = APP_NAME;
        msg.appdata.appversion = APP_VERSION;
        msg.timestamp.utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.timestamp.user = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.userdata = userdata;
        return UnityEngine.JsonUtility.ToJson(msg);
    }
/*
    public static string WrapUserData(IcCrashLogData crashdata) {
        IcarusMsg msg = new IcarusMsg<IcCrashLogData>();
        msg.debug = true;
        msg.appdata.uuid = GetUUID();
        msg.appdata.voiceover = false;
        msg.appdata.lang = L10N.CurrentLanguage.ToString();
        msg.appdata.device = SystemInfo.deviceModel;
        msg.appdata.appname = "qU";
        msg.appdata.appversion = APP_VERSION;
        msg.timestamp.utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.timestamp.user = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.userdata = crashdata;

        return UnityEngine.JsonUtility.ToJson(msg);
    }*/

    private static string GetUUID() {
        string uuid = PlayerPrefs.GetString(UUID_KEY, "");
        if (uuid.Length == 0) {
            uuid = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(UUID_KEY, uuid);
        }
        return uuid;
    }
}
