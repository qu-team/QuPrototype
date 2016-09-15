using UnityEngine;
using System.Threading;

public class HarvestThreadLauncher : MonoBehaviour {
    static bool threadWasStarted = false;
    static bool threadWasStopped = false;

    Thread harvestThread;

    void Awake() {
        if (!threadWasStarted) {
            threadWasStarted = true;
            harvestThread = new Thread(HarvesterDaemon.Instance.Run);
            harvestThread.IsBackground = true;
            harvestThread.Name = "Harvester";
            harvestThread.Start();
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnApplicationQuit() {
        if (!threadWasStopped) {
            threadWasStopped = true;
            Debug.Log("Stopping harvester");
            HarvesterDaemon.Instance.Stop();
            // Wait until the thread actually exits
            lock (HarvesterDaemon.Instance.stopHandle)
                if (!Monitor.Wait(HarvesterDaemon.Instance.stopHandle, 3000))
                    harvestThread.Abort();
        }
    }
}
