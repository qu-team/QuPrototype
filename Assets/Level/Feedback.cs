using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Feedback : MonoBehaviour {

    public RectTransform ok;
    public RectTransform wrong;

    void Start() {
        ok.gameObject.SetActive(false);
        ok.localScale = Vector3.zero;
        wrong.gameObject.SetActive(false);
        wrong.localScale = Vector3.zero;
    }

    public void Ok() {
        StartCoroutine(OkAnimation());
    }

    public void Wrong() {
        StartCoroutine(WrongAnimation());
    }

    IEnumerator OkAnimation() {
        ok.localScale = new Vector3(1f, 1f, 1f);
        ok.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        float size = 1f;
        while (size >= 0f) {
            size -= Time.deltaTime * 5f;
            if (size > 0) {
                ok.localScale = new Vector3(size, size, 1f);
                yield return null;
            } else {
                ok.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator WrongAnimation() {
        wrong.localScale = new Vector3(1f, 1f, 1f);
        wrong.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        float size = 1f;
        while (size >= 0f) {
            size -= Time.deltaTime * 5f;
            if (size > 0) {
                wrong.localScale = new Vector3(size, size, 1f);
                yield return null;
            } else {
                wrong.gameObject.SetActive(false);
            }
        }
    }
}
