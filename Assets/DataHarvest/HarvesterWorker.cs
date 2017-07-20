using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_WSA
    using System.Linq;
#endif

internal sealed class HarvesterWorker {

    readonly string SERVER_ADDRESS = "http://127.0.0.1";
    readonly int SERVER_PORT = 8000;
    readonly string REQUEST_PATH = "/";
    readonly string REQUEST_URL;

    LocalDataHandler localData;
    readonly string path;

    public HarvesterWorker() {
        path = Application.persistentDataPath;
        try {
            LogHelper.Debug(this, "loading Resources/appconfig.json");
            var conf = AppConfig.FromResources("appconfig");
            if (!conf.forceLocalhost) {
                SERVER_ADDRESS = conf.dataserver.address;
                SERVER_PORT = conf.dataserver.port;
                REQUEST_PATH = conf.dataserver.path;
            }
        } catch (Exception ex) {
#if UNITY_EDITOR
            LogHelper.Warn(this, "AppConfig wasn't loaded correctly from " + 
                    Application.dataPath + "/appconfig.json:\n" + ex);
#else
            var buttons = GameObject.Find("Buttons");
	    if (buttons != null) buttons.SetActive(false);
            GameObject.Find("Loading").GetComponent<UnityEngine.UI.Text>().text = "appconfig not found.";
	    Time.timeScale = 0f;
#endif
        }
	LogHelper.Ok(this, "appconfig loaded correctly.");
        REQUEST_URL = SERVER_ADDRESS + ":" + SERVER_PORT + REQUEST_PATH;
        LogHelper.Info(this, "request url: " + REQUEST_URL);
        localData = new LocalDataHandler(path);
    }

    // Coroutine which tries to send `data` to the server.
    // If `sourceFile` is given, the data is assumed to come from that file, therefore
    // that file will be deleted after a successful send operation, and no local file will be
    // written if said operation fails.
    public IEnumerator SendData(List<DataBundle> data, string sourceFile = null) {
        LogHelper.Info(this, "sending data to " + REQUEST_URL + "...");

        string alldata = Protocol.WrapUserData(data);
        byte[] payload = Encoding.UTF8.GetBytes(alldata);
        var request = new UnityWebRequest(REQUEST_URL);
        request.method = "POST";
        UploadHandler uploader = new UploadHandlerRaw(payload);
        request.uploadHandler = uploader;
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

#if UNITY_5
        if (!request.isError) {
#else
        if (!request.isNetworkError) {
#endif
            LogHelper.Ok(this, "data sent successfully");
            if (sourceFile != null)
                File.Delete(sourceFile);
        } else {
            LogHelper.Warn(this, "error: " + request.error);
            LogHelper.Info(this, "saving data locally");
            // Only save locally if this data doesn't already come from a local file
            if (sourceFile == null)
                localData.SaveCompressed(Data.Serialize(data));
        }

        yield return null;
    }

    // Coroutine which checks local data directory and tries to send all data found inside.
    public IEnumerator SendLocal() {
        LogHelper.Info(this, "checking for local data...");

#if UNITY_WSA
        string[] files = new List<string>(Directory.GetFiles(path)).Where(file => {
                return file.StartsWith("qudata_") && file.EndsWith(".gz");
        }).ToArray();
#else
        string[] files = Directory.GetFiles(path, "qudata_*.gz");
#endif
        foreach (string fname in files) {
            LogHelper.Debug(this, "loading file " + fname);
            string datastr = localData.LoadCompressed(fname);
            if (datastr == null)
                continue;

            LogHelper.Debug(this, "data string = " + datastr);
            var data = Data.Deserialize(datastr);
            LogHelper.Debug(this, "loaded data: " + data);
            if (data == null) {
                LogHelper.Info(this, "data in file " + fname + " is unparseable or has an " +
                               "incompatible version: deleting.");
                File.Delete(fname);
            } else if (data.Count == 0) {
                LogHelper.Warn(this, "local file " + fname + " was empty");
                File.Delete(fname);
            } else {
                yield return SendData(data, fname);
            }
        }

        yield return null;
    }
}
