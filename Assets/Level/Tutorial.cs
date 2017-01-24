using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    bool listening;
    Level level;
    GameObject hand;
    GameObject arrow;

    void Awake() {
        hand = GameObject.Find("Hand");
        arrow = GameObject.Find("Arrow");
        hand.SetActive(false);
        arrow.SetActive(false);

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
        if (!level.Paused && listening && level.shutter.opening <= 0.1f) {
            level.Pause();
            StartCoroutine(ShowColorEquality());
        }
    }

    void Continue(Color color) {
        if (listening) { return; }
        level.Resume();
        level.MatchQuColor(color);
        listening = true;
        hand.SetActive(false);
        print("eactivated hand");
    }

    IEnumerator ShowColorEquality() {
        SetArrowAtCorrectColor();
        arrow.SetActive(true);
        yield return new WaitForSeconds(1f);
        arrow.SetActive(false);
        SetHandAtCorrectColor();
        hand.SetActive(true);
        yield return null;
        listening = false;
    }

    // Warning: this only works if there are exactly 3 blades!
    void SetArrowAtCorrectColor() {
        foreach (var blade in level.shutter.blades) {
            if (blade.GetComponentInChildren<MeshRenderer>().material.color == level.qu.Color) {
                var bpos = blade.transform.FindChild("Shape").position;
                var qpos = level.qu.transform.position;
                float coef = bpos.y > qpos.y
                             ? 0.8f // Upper blade
                             : bpos.x > qpos.x
                                ? 0.8f // Lower blade
                                : 0.8f // Left blade
                             ;
                arrow.transform.position = bpos + (qpos - bpos) * coef + new Vector3(0, 0, -3);
                float angle = bpos.y > qpos.y
                             ? 15f // Upper blade
                             : bpos.x > qpos.x
                                ? 80f // Lower blade
                                : 130f // Left blade
                             ;
                arrow.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                return;
            }
        }
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
