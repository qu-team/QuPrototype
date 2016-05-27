using UnityEngine;
using System.Collections;

public class ThreeBladesLevel : MonoBehaviour {

    public Shutter shutter;
    public float closingSpeed;

    const float SIZE = 6f;
    const float MAX_OPENING = 0.25f;
    const float MIN_OPENING = 0.04f;

    bool finalClosing = false;

    void Awake() {
        shutter.bladesNumber = 3;
        shutter.relativeSize = SIZE;
        shutter.opening = MAX_OPENING;
    }

    void Update() {
        if (finalClosing) { return; }
        if (shutter.opening == 0f) {
            shutter.opening = MAX_OPENING;
        } else if (shutter.opening <= MIN_OPENING) {
            finalClosing = true;
            StartCoroutine(FinalClosingAnimation());
        } else {
            shutter.opening -= closingSpeed * Time.deltaTime;
        }
    }

    IEnumerator FinalClosingAnimation() {
        yield return new WaitForSeconds(0.5f);
        while (shutter.opening >= 0.0001f) {
            shutter.opening /= 1.1f;
            yield return null;
        }
        shutter.opening = 0f;
        finalClosing = false;
    }
}
