using UnityEngine;

public class KeepScreenOn : MonoBehaviour {

    void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
