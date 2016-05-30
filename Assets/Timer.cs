using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public float remainingTime = 60f;
    public Text timerBoard;

    public bool OutOfTime { get { return remainingTime <= 0f; } }

    void Start() {
        UpdateTimerBoard();
    }

    public void Restart(float time) {
        remainingTime = time;
        UpdateTimerBoard();
    }

    void Update() {
        if (remainingTime <= 0f) { return; }
        remainingTime -= Time.deltaTime;
        if (remainingTime < 0f) { remainingTime = 0f; }
        UpdateTimerBoard();
    }

    void UpdateTimerBoard() {
        if (timerBoard == null) { return; }
        if (remainingTime == 0f) {
            timerBoard.text = "0.0";
        } else if (remainingTime < 10f) {
            if (timerBoard.color != Color.red) { timerBoard.color = Color.red; }
            timerBoard.text = string.Format("{0}:n1", remainingTime);
        } else {
            if (timerBoard.color != Color.white) { timerBoard.color = Color.white; }
            timerBoard.text = ((int)remainingTime).ToString();
        }
    }
}
