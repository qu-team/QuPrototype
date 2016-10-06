using UnityEngine.Networking;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;

public sealed class HarvesterClient {
    
    const string SERVER_ADDRESS = "http://159.149.142.22";
    const int SERVER_PORT = 8000;
    const string REQUEST_PATH = "/";
    const float CONN_TIMEOUT = 5f;

    static readonly string REQUEST_URL = SERVER_ADDRESS + ":" + SERVER_PORT + REQUEST_PATH;
    
    // Tries to send `data` to the server (synchronously). Returns success or failure status.
    public IEnumerator SendData(List<DataBundle> data) {
        LogHelper.Info(this, "sending data to " + REQUEST_URL + "...");
        LogHelper.Debug(this, "data has " + data.Count + " elements");
        
        string alldata = Protocol.WrapUserData(data);
        LogHelper.Debug(this, "Sending payload:\n" + alldata);
        byte[] payload = Encoding.UTF8.GetBytes(alldata);
        var request = new UnityWebRequest(REQUEST_URL);
        request.method = "POST";
        UploadHandler uploader = new UploadHandlerRaw(payload);
        uploader.contentType = "application/json";
        request.uploadHandler = uploader;
        
        yield return request.Send();

        if (!request.isError) {
            LogHelper.Ok(this, "data sent successfully");
        } else {
            LogHelper.Warn(this, "error: " + request.error);
        }
    }
}
