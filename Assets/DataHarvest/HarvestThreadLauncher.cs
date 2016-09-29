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
        if (!threadWasStopped && HarvesterDaemon.Instance.IsRunning) {
            threadWasStopped = true;
            LogHelper.Info(this, "stopping harvester.");
            HarvesterDaemon.Instance.Stop();
            // Wait until the thread actually exits
            lock (HarvesterDaemon.Instance.stopHandle) {
                if (!Monitor.Wait(HarvesterDaemon.Instance.stopHandle, 3000)) {
                    LogHelper.Warn(this, "stopping thread timed out.");
                    harvestThread.Abort();
                }
            }
        }
    }
}
