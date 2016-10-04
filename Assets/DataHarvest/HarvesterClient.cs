using UnityEngine.Networking;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System;
using System.Collections;

public sealed class HarvesterClient {
    
    const string SERVER_ADDRESS = "http://127.0.0.1";
    const int SERVER_PORT = 8000;
    const string REQUEST_PATH = "/";
    const float CONN_TIMEOUT = 5f;

    static readonly string REQUEST_URL = SERVER_ADDRESS + ":" + SERVER_PORT + REQUEST_PATH;
    
    // Tries to send `data` to the server (synchronously). Returns success or failure status.
    public IEnumerator SendData(string data) {
        LogHelper.Info(this, "sending data to " + REQUEST_URL + "...");
        
        string alldata = Protocol.WrapUserData(data);
        byte[] payload = Encoding.UTF8.GetBytes(alldata);
        var request = new UnityWebRequest(REQUEST_URL);
        UploadHandler uploader = new UploadHandlerRaw(payload);
        uploader.contentType = "application/json";
        request.uploadHandler = uploader;
        
        yield return request.Send();
    }
}
