using UnityEngine;
using System.Collections;

public class Qu : MonoBehaviour {

    public GameObject eyes;
    public GameObject stretchedEyes;
    public GameObject happyEyes;
    public GameObject scaredEyes;

    SpriteRenderer sprite;
    Vector3 originalScale;
    bool dead = false;
    int expressionCount = 0;
    IEnumerator deathCoroutine;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        DisableEyes();
        eyes.SetActive(true);
    }

    void Start() {
        StartCoroutine(MoveEyes());
    }

    public Color Color {
        get { return sprite.color; }
        set {
            sprite.color = value;
            SetEyesColor(white: value.r < 0.5f && value.g < 0.5f && value.b < 0.5f);
        }
    }

    void SetEyesColor(bool white) {
        foreach (var eyesKind in new GameObject[] { eyes, stretchedEyes, happyEyes }) {
            foreach (var eye in eyesKind.GetComponentsInChildren<SpriteRenderer>(includeInactive: true)) {
                eye.color = white ? Color.white : Color.black;
            }
        }
    }

    public void Die() {
        dead = true;
        deathCoroutine = DeathAnimation();
        StartCoroutine(deathCoroutine);
    }

    public void StopDying() {
        if (deathCoroutine != null) {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
        }
    }

    public bool Dead { get { return dead; } }

    public void Restore() {
        transform.localScale = originalScale;
        dead = false;
    }

    public void StretchEyes() {
        DisableEyes();
        stretchedEyes.SetActive(true);
    }

    public void OpenEyes() {
        DisableEyes();
        eyes.SetActive(true);
    }

    public void BeHappy() {
        StartCoroutine(ChangeEyes(happyEyes, 1f));
    }

    public void BeScared() {
        StartCoroutine(ChangeEyes(scaredEyes, 1f));
    }

    void DisableEyes() {
        eyes.SetActive(false);
        stretchedEyes.SetActive(false);
        happyEyes.SetActive(false);
        scaredEyes.SetActive(false);
    }

    IEnumerator DeathAnimation() {
        while (transform.localScale.x >= 0.001f) {
            transform.localScale /= 1.2f;
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }

    IEnumerator MoveEyes() {
        while (isActiveAndEnabled) {
            if (Random.value < 0.02f) {
                var r = Random.value * 5f;
                var a = Random.value * Mathf.PI * 2f;
                eyes.transform.localPosition = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
                yield return new WaitForSeconds(0.5f);
            } else {
                yield return null;
            }
        }
    }

    IEnumerator ChangeEyes(GameObject eyesKind, float duration) {
        var expressionId = ++expressionCount;
        DisableEyes();
        eyesKind.SetActive(true);
        while (duration > 0f) {
            duration -= Time.deltaTime;
            yield return null;
        }
        if (expressionId == expressionCount) { OpenEyes(); }
    }
}
