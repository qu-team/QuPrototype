using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

// The HarvesterDaemon runs its Run method in a separate thread
// to perform blocking operations like file/network IO.
public class HarvesterDaemon {
    
    private static HarvesterDaemon instance;

    public static HarvesterDaemon Instance {
        get {
            if (instance == null)
                instance = new HarvesterDaemon();
            return instance;
        }
    }

    public Queue<IEnumerable<DataBundle>> dataPipe;
    public object stopHandle;

    bool shouldStop = false;

    private HarvesterDaemon() {
        dataPipe = new Queue<IEnumerable<DataBundle>>();
        stopHandle = new object();
    }

    // Method called when thread starts
    public void Run() {
        // TODO check for local files and try sending them over network
    
        Debug.Log("Harvester thread started");

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

            } catch (InvalidOperationException ex) {
                // just skip this
                Debug.Log(ex.StackTrace);
            }
        }

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

    private string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("{0}-{1}-{2}T{3}:{4}:{5}_{6}", 
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }
}
