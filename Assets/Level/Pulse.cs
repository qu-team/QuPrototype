using UnityEngine;

public class Pulse : MonoBehaviour {

    public float amplitude;
    public float frequency;

    Vector3 originalScale;

    void Start() {
        originalScale = transform.localScale;
    }

	void Update () {
        transform.localScale = originalScale + new Vector3(1f, 1f, 1f) * amplitude * Mathf.Sin(Time.time * frequency * 2f * Mathf.PI);
	}
}
