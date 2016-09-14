using UnityEngine;

public class Timer : MonoBehaviour {

    public RectTransform timerBar;

    float totalTime;
    float remainingTime;

    public bool OutOfTime { get { return remainingTime <= 0f; } }

    void Start() {
        UpdateTimerBoard();
    }

    public void Set(float time) {
        totalTime = time;
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
        if (timerBar == null) { return; }
        if (remainingTime == 0f) {
            timerBar.anchorMax = new Vector2(0f, timerBar.anchorMax.y);
        } else {
            timerBar.anchorMax = new Vector2(remainingTime / totalTime, timerBar.anchorMax.y);
        }
    }
}
