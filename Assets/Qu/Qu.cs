using UnityEngine;
using System.Collections;

public class Qu : MonoBehaviour {

    public GameObject eyes;
    public GameObject stretchedEyes;

    SpriteRenderer sprite;
    Vector3 originalScale;
    bool dead = false;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        stretchedEyes.SetActive(false);
    }

    void Start() {
        StartCoroutine(MoveEyes());
    }

    public Color Color {
        get { return sprite.color; }
        set { sprite.color = value; }
    }

    public void Die() {
        dead = true;
        StartCoroutine(DeathAnimation());
    }

    public bool Dead { get { return dead; } }

    public void Restore() {
        OpenEyes();
        transform.localScale = originalScale;
        dead = false;
    }

    public void StretchEyes() {
        eyes.SetActive(false);
        stretchedEyes.SetActive(true);
    }

    public void OpenEyes() {
        stretchedEyes.SetActive(false);
        eyes.SetActive(true);
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
            }
            yield return null;
        }
    }
}
