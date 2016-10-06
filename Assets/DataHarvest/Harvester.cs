using UnityEngine;
using System;
using System.Collections.Generic;

// Harvester is the main entrypoint of the data harvesting module.
public sealed class Harvester {

    private List<DataBundle> storedData;
    private HarvesterWorker worker;

    public Harvester(HarvesterDaemon daemon) {
        storedData = new List<DataBundle>();
        worker = new HarvesterWorker();
    }

    // Saves the data from a single tap-to-tap session and stores it into memory.
    public void SaveSingleSessionData(Level level, bool succeeded) {
        storedData.Add(Data.Create(level, succeeded));
    }

    public void SendStoredData(MonoBehaviour mb) {
        // Try sending data over the network, save locally if unable to send.
        mb.StartCoroutine(worker.SendData(new List<DataBundle>(storedData)));
        storedData.Clear();
    }
}
