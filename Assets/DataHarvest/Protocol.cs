using System;
using System.Globalization;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct IcarusMsg {
    public IcAppdata appdata;
    public bool debug;
    public IcQuUserdata userdata;
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

/* The data structure is:
 * {
 *	"appdata": IcAppdata
 *	"debug": bool
 * 	"userdata": {
 *		"devicedata": Devicedata
 *		"gamedata": List<DataBundle>
 *	}
 *	"timestamp": IcTimestamp
 * }
 */
public static class Protocol {
    // Adds the JSON fields required by the Icarus server protocol to `gamedata`.
    public static string WrapUserData(List<DataBundle> gamedata) {
        IcarusMsg msg = new IcarusMsg();
        msg.debug = true;
        msg.appdata.uuid = "NOT_IMPLEMENTED";
        msg.appdata.voiceover = false;
        msg.appdata.lang = Application.systemLanguage.ToString();
        msg.appdata.device = SystemInfo.deviceModel;
        msg.appdata.appname = "Qu";
        msg.appdata.appversion = 1;
        msg.timestamp.utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.timestamp.user = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        msg.userdata = new IcQuUserdata {
            gamedata = gamedata,
            devicedata = new Devicedata {
                screenDPI = Screen.dpi,
                screenHeight = Screen.height,
                screenWidth = Screen.width
            }
        };

        return UnityEngine.JsonUtility.ToJson(msg);
    }
}
