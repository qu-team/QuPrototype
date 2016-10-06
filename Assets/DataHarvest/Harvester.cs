using UnityEngine;
using System;
using System.Collections.Generic;

// Harvester is the object acting as an interface to the harvester thread.
// All its methods are synchronous and execute on the main thread, but it
// communicates with the harvester thread via a queue.
public sealed class Harvester {

    private List<DataBundle> storedData;
    private HarvesterDaemon daemon;
    private HarvesterClient client;

    public Harvester(HarvesterDaemon daemon) {
        storedData = new List<DataBundle>();
        client = new HarvesterClient();
        this.daemon = daemon;
    }

    // Saves the data from a single tap-to-tap session and stores it into memory.
    public void SaveSingleSessionData(Level level, bool succeeded) {
        storedData.Add(Data.Create(level, succeeded));
    }

    public void SendStoredData(MonoBehaviour mb) {
        // Serialize data
        string json = Data.Serialize(storedData);

        // Tell the daemon to save data locally
        daemon.dataPipe.Enqueue(json);

        // Try sending data over the network
        mb.StartCoroutine(client.SendData(new List<DataBundle>(storedData)));

        storedData.Clear();
    }
}
