using UnityEngine;
using System.Collections;

public class ThreeBladesLevel : MonoBehaviour {

    public Shutter shutter;
    public float closingSpeed;

    const float SIZE = 6f;
    const float MAX_OPENING = 0.25f;

    void Awake() {
        shutter.bladesNumber = 3;
        shutter.relativeSize = SIZE;
        shutter.opening = MAX_OPENING;
    }

    void Update() {
        if (shutter.opening == 0) {
            shutter.opening = MAX_OPENING;
        } else {
            shutter.opening -= closingSpeed * Time.deltaTime;
        }
    }
}
