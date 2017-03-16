using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathRing : MonoBehaviour {

    Image image;
    IEnumerator closeCoroutine;

    // Use this for initialization
    void Start() {
        image = GetComponent<Image>();
        var level = GameObject.FindObjectOfType<Level>();
        level.OnFinalClosing += StartClosing;
        level.OnStart += Reset;
        level.OnReinitialize += Reset;
        level.OnOutOfTime += Reset;
    }

    void StartClosing(float duration) {
        StartCoroutine(closeCoroutine = Close(duration));
    }

    void Reset() {
        if (closeCoroutine != null) {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }
        image.fillAmount = 1f;
        var c = image.color;
        image.color = new Color(c.r, c.g, c.b, 0f);
    }

    IEnumerator Close(float duration) {
        var c = image.color;
        image.color = new Color(c.r, c.g, c.b, 1f);
        while (image.fillAmount > 0) {
            image.fillAmount -= Time.deltaTime / duration;
            yield return null;
        }
    }
}
