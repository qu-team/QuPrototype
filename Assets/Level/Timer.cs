﻿using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public RectTransform timerBar;
    public Text timerText;

    float totalTime;
    float remainingTime;

    public bool OutOfTime { get { return remainingTime <= 0f; } }
    public float TimeSinceStart { get { return totalTime - remainingTime; } }

    void Start() {
        UpdateTimerBoard();
    }

    public void Set(float time) {
        totalTime = time;
        remainingTime = time;
        UpdateTimerBoard();
    }

    public void TimePenality(float seconds) {
        if (seconds <= 0f) { return; }
        remainingTime -= seconds;
        if (remainingTime < 0f) { remainingTime = 0f; }
        UpdateTimerBoard();
    }

    void Update() {
        if (remainingTime <= 0f) { return; }
        remainingTime -= Time.deltaTime;
        if (remainingTime < 0f) { remainingTime = 0f; }
        UpdateTimerBoard();
        UpdateTimerText();
    }

    void UpdateTimerBoard() {
        if (timerBar == null) { return; }
        if (remainingTime == 0f) {
            timerBar.anchorMax = new Vector2(0f, timerBar.anchorMax.y);
        } else {
            timerBar.anchorMax = new Vector2(remainingTime / totalTime, timerBar.anchorMax.y);
        }
    }

    void UpdateTimerText() {
        timerText.text = string.Format("{0}", (int)remainingTime);
    }
}
