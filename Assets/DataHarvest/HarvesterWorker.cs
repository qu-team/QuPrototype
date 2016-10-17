using UnityEngine;
using UnityEngine.Networking;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;

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
            LogHelper.Debug(this, "loading " + Application.dataPath + Path.DirectorySeparatorChar + "appconfig.json");
            var conf = AppConfig.FromFile(Application.dataPath + Path.DirectorySeparatorChar + "appconfig.json");
            if (!conf.debug) {
                SERVER_ADDRESS = conf.dataserver.address;
                SERVER_PORT = conf.dataserver.port;
                REQUEST_PATH = conf.dataserver.path;
            }
        } catch (Exception ex) {
            LogHelper.Warn(this, ex.StackTrace);
        }
        REQUEST_URL = SERVER_ADDRESS + ":" + SERVER_PORT + REQUEST_PATH;
        LogHelper.Info(this, "request url: " + REQUEST_URL);
        localData = new LocalDataHandler(path);
    }

    // Tries to send `data` to the server.
    // If `sourceFile` is given, the data is assumed to come from that file, therefore
    // that file will be deleted after a successful send operation, and no local file will be
    // written if said operation fails.
    // Returns success or failure status.
    public IEnumerator SendData(List<DataBundle> data, string sourceFile = null) {
        LogHelper.Info(this, "sending data to " + REQUEST_URL + "...");

        string alldata = Protocol.WrapUserData(data);
        byte[] payload = Encoding.UTF8.GetBytes(alldata);
        var request = new UnityWebRequest(REQUEST_URL);
        request.method = "POST";
        UploadHandler uploader = new UploadHandlerRaw(payload);
        uploader.contentType = "application/json";
        request.uploadHandler = uploader;

        yield return request.Send();

        if (!request.isError) {
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

    // Checks local data directory and tries to send all data found inside.
    public IEnumerator SendLocal() {
        LogHelper.Info(this, "checking for local data...");

        foreach (string fname in Directory.GetFiles(path, "qudata_*.gz")) {
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
