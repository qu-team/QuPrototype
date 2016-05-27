using UnityEngine;
using System.Collections;

public class Qu : MonoBehaviour {

    public GameObject eyes;

    SpriteRenderer sprite;
    Vector3 originalScale;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    void Start() {
        StartCoroutine(MoveEyes());
    }

    public void SetColor(Color color) {
        sprite.color = color;
    }

    public void Die() {
        StartCoroutine(DeathAnimation());
    }

    public void Restore() {
        transform.localScale = originalScale;
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
