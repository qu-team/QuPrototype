using UnityEngine;
using UnityEngine.Networking;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;

internal sealed class HarvesterWorker {
    
    const string SERVER_ADDRESS = "http://127.0.0.1";
    const int SERVER_PORT = 8000;
    const string REQUEST_PATH = "/";
    const float CONN_TIMEOUT = 5f;

    static readonly string REQUEST_URL = SERVER_ADDRESS + ":" + SERVER_PORT + REQUEST_PATH;

    LocalDataHandler localData;
    readonly string path;

    public HarvesterWorker() {
        path = Application.persistentDataPath;
        localData = new LocalDataHandler(path);
    }
    
    // Tries to send `data` to the server (synchronously). 
    // If `fileToRemove` is given, it'll be deleted after a successful send operation.
    // Returns success or failure status.
    public IEnumerator SendData(List<DataBundle> data, string fileToRemove = null) {
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
            if (fileToRemove != null)
                File.Delete(fileToRemove);
        } else {
            LogHelper.Warn(this, "error: " + request.error);
            LogHelper.Info(this, "saving data locally");
            localData.SaveCompressed(Data.Serialize(data));
        }

        yield return null;
    }

    // Checks local data directory and tries to send all data found inside.
    public IEnumerator SendLocal() {
        LogHelper.Info(this, "checking for local data...");

        foreach (string fname in Directory.GetFiles(path, "*.gz")) {
            LogHelper.Debug(this, "loading file " + fname);
            string datastr = localData.LoadCompressed(fname);
            if (datastr == null)
                continue;

            LogHelper.Debug(this, "data string = " + datastr);
            var data = Data.Deserialize(datastr);
            LogHelper.Debug(this, "loaded data: " + data);
            if (data == null) {
                LogHelper.Info(this, "data in file " + fname + " has an incompatible version: deleting.");
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
