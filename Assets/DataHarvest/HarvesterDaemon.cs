using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

// The HarvesterDaemon runs its Run method in a separate thread
// to perform blocking operations like file/network IO.
public sealed class HarvesterDaemon {
    
    private static HarvesterDaemon instance;

    public static HarvesterDaemon Instance {
        get {
            if (instance == null)
                instance = new HarvesterDaemon(Application.persistentDataPath);
            return instance;
        }
    }

    public bool IsRunning { get { return running; } }

    public Queue<IEnumerable<DataBundle>> dataPipe;
    public object stopHandle;

    bool shouldStop = false;
    bool running = false;
    HarvesterClient client;
    LocalDataHandler localData;

    private HarvesterDaemon(string persistentDataPath) {
        dataPipe = new Queue<IEnumerable<DataBundle>>();
        stopHandle = new object();
        localData = new LocalDataHandler(persistentDataPath);
    }

    // Method called when thread starts
    public void Run() {
        if (!StartClient()) {
            LogHelper.Error(this, "failed to start.");
            return;
        }
        // TODO check for local files and try sending them over network
    
        LogHelper.Info(this, "thread started.");
        running = true;

        while (!shouldStop) {
            if (dataPipe.Count == 0) {
                Thread.Sleep(1000);
                continue;
            }

            try {
                // Serialize data into JSON
                string json = SerializeData(dataPipe.Dequeue());
                if (json == null) continue;

                Debug.Log(json);
                // Send data to the server
                if (client.SendData(json)) {
                    LogHelper.Info(this, "data sent successfully");
                } else {
                    LogHelper.Info(this, "network is not available: saving data locally");
                    localData.SaveLocally(json);
                }

            } catch (InvalidOperationException ex) {
                // just skip this
                LogHelper.Error(this, ex.StackTrace);
            }
        }

        LogHelper.Info(this, "notifying other threads about our stop...");
        lock (stopHandle)
            Monitor.PulseAll(stopHandle);
    }

    public void Stop() {
        shouldStop = true;
    }

    private string SerializeData(IEnumerable<DataBundle> dataList) {
        StringBuilder jsonBuilder = new StringBuilder("[");
        foreach (DataBundle data in dataList) {
            jsonBuilder.Append(UnityEngine.JsonUtility.ToJson(data));
            jsonBuilder.Append(",");
        }

        if (jsonBuilder.Length == 1) return null; // should never happen

        // remove trailing comma
        jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
        jsonBuilder.Append("]");

        return jsonBuilder.ToString();
    }

    private bool StartClient() {
        try {
            client = new HarvesterClient();
        } catch (Exception ex) {
            LogHelper.Error(this, ex.StackTrace);
            return false;
        }
        return true;
    }
}
