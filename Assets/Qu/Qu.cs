using UnityEngine;
using System.Collections;

public class Qu : MonoBehaviour {

    public GameObject eyes;

    SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(MoveEyes());
    }

    public void SetColor(Color color) {
        sprite.color = color;
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
