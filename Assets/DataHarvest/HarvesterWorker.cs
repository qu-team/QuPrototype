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

    LocalDataHandler localData;
    readonly string path;
    readonly string REQUEST_URL;

    public HarvesterWorker() {
        path = Application.persistentDataPath;
        var conf = GameManager.Instance.AppConfig;
        REQUEST_URL = conf.GetRequestURL();
        LogHelper.Info(this, "request url: " + REQUEST_URL);
        localData = new LocalDataHandler(path);
    }

    // Coroutine which tries to send `data` to the server.
    // If `sourceFile` is given, the data is assumed to come from that file, therefore
    // that file will be deleted after a successful send operation, and no local file will be
    // written if said operation fails.
    public IEnumerator SendData(List<DataBundle> data, string sourceFile = null) {
        LogHelper.Info(this, "sending data to " + REQUEST_URL + "...");

        string alldata = Protocol.WrapUserData(new IcQuUserdata {
            gamedata = data,
            devicedata = new Devicedata {
                screenDPI = Screen.dpi,
                screenHeight = Screen.height,
                screenWidth = Screen.width
            }
        });

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
            if (datastr == null) {
                LogHelper.Info(this, "data in file " + fname + " is empty: deleting.");
                File.Delete(fname);
                continue;
            }

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
