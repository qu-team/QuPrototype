using UnityEngine;
using UnityEngine.UI;

public class MaxScore : MonoBehaviour {

    uint lastScore = 0;

    void Update() {
        if (lastScore == GameData.MaxScore) { return; }
        GetComponent<Text>().text = string.Format("RECORD: {0}", GameData.MaxScore);
        lastScore = GameData.MaxScore;
    }
}
