using UnityEngine;
using System;
using System.Collections.Generic;

// Harvester is the main entrypoint of the data harvesting module.
public sealed class Harvester {

    private static Harvester instance;

    public static Harvester Instance {
        get {
            if (instance == null)
                instance = new Harvester();
            return instance;
        }
    }

    private List<DataBundle> storedData;
    private HarvesterWorker worker;

    private Harvester() {
        storedData = new List<DataBundle>();
        worker = new HarvesterWorker();
    }

    // If we have local files, try sending them over the network and remove the sent ones.
    public void SendLocalData(MonoBehaviour mb) {
        mb.StartCoroutine(worker.SendLocal());
    }

    // Saves the data from a single tap-to-tap session and stores it into memory.
    public void SaveSingleSessionData(Level level, bool succeeded) {
        storedData.Add(Data.Create(level, succeeded));
    }

    // Try sending data over the network, save locally if unable to send.
    public void SendStoredData(MonoBehaviour mb) {
        mb.StartCoroutine(worker.SendData(new List<DataBundle>(storedData)));
        storedData.Clear();
    }
}
