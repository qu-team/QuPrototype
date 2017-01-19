using UnityEngine;

public class Tutorial : MonoBehaviour {

    bool listening;
    Level level;
    GameObject hand;

    void Awake() {
        hand = transform.FindChild("Hand").gameObject;
        hand.SetActive(false);

        // disable score
        GameObject.Find("Score").SetActive(false);

        level = GameObject.FindObjectOfType<Level>();
        level.IsTutorial = true;
        listening = true;

        // prevent clicking before stopping the level
        level.shutter.OnColorSelected -= level.MatchQuColor;
        // inject our event handler
        level.shutter.OnColorSelected += Continue;
    }

    void Update() {
        if (listening && level.shutter.opening <= 0.1f) {
            listening = false;
            SetHandAtCorrectColor();
            hand.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void Continue(Color color) {
        if (Time.timeScale != 0f) return;
        Time.timeScale = 1f;
        level.MatchQuColor(color);
        listening = true;
        hand.SetActive(false);
    }

    // Warning: this only works if there are exactly 3 blades!
    void SetHandAtCorrectColor() {
        foreach (var blade in level.shutter.blades) {
            if (blade.GetComponentInChildren<MeshRenderer>().material.color == level.qu.Color) {
                var bpos = blade.transform.FindChild("Shape").position;
                var qpos = level.qu.transform.position;
                float coef = bpos.y > qpos.y
                             ? 0.5f // Upper blade
                             : bpos.x > qpos.x
                                ? 0.7f // Lower blade
                                : 0.5f // Left blade
                             ;
                hand.transform.position = bpos + (qpos - bpos) * coef + new Vector3(0, 0, -3);
                return;
            }
        }
    }
}
