using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

// The HarvesterDaemon is a singleton that runs its Run method in a separate thread
// to perform blocking operations like file IO.
// Basically, it periodically polls its data pipe and saves all its element
// to separate local files via LocalDataHandler.
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

    // Contains the text to save locally
    public Queue<string> dataPipe;
    public object stopHandle;

    bool shouldStop = false;
    bool running = false;
    LocalDataHandler localData;

    private HarvesterDaemon(string persistentDataPath) {
        dataPipe = new Queue<string>();
        stopHandle = new object();
        localData = new LocalDataHandler(persistentDataPath);
    }

    // Method called when thread starts
    public void Run() {
        LogHelper.Info(this, "thread started.");
        running = true;

        while (!shouldStop) {
            if (dataPipe.Count == 0) {
                Thread.Sleep(1000);
                continue;
            }

            try {
                string json = dataPipe.Dequeue();
                LogHelper.Info(this, "saving data locally");
                localData.SaveCompressed(json);

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
}
