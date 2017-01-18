using UnityEngine;

public class Tutorial : MonoBehaviour {

    bool listening;
    Shutter shutter;
    Level level;

    void Start() {
        shutter = GameObject.FindObjectOfType<Shutter>();
        level = GameObject.FindObjectOfType<Level>();
        listening = true;
        // TODO: prevent clicking before stopping the level;
        // inject our event handler
        shutter.OnColorSelected -= level.MatchQuColor;
    }

    void Update() {
        LogHelper.Debug(this, "shutter.opening = " + shutter.opening);
        if (listening && shutter.opening <= 0.1f) {
            // FIXME: not called
            shutter.OnColorSelected += Continue;
            LogHelper.Debug(this, "Stopping...");
            listening = false;
            Time.timeScale = 0f;
        }
    }

    private void Continue(Color color) {
        Time.timeScale = 1f;
        shutter.OnColorSelected -= Continue;
    }
}
