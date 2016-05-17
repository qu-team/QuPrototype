using UnityEngine;

public class Shutter : MonoBehaviour {

    [RangeAttribute(0f, 1f)]
    public float opening = 1f;
    public uint bladesNumber = 6;

    GameObject[] blades;
    float lastOpening = 1f;

    void Awake() {
        blades = new GameObject[bladesNumber];
        var portion = Mathf.PI * 2 / bladesNumber;
        for (int i = 0; i < blades.Length; i++) {
            var angle = (i + 0.5f) * (360f / bladesNumber);
            var position = new Vector2(Mathf.Cos(i * portion), Mathf.Sin(i * portion));
            blades[i] = new GameObject("Blade" + i);
            blades[i].transform.parent = transform;
            blades[i].transform.localPosition = position;
            blades[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            var sprite = new GameObject("Sprite");
            sprite.transform.parent = blades[i].transform;
            sprite.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            var triangle = sprite.AddComponent<Triangle>();
            triangle.topAngle = portion;
            sprite.transform.localPosition = new Vector2(0f, -triangle.Width / 2);
        }
    }

    void Update() {
        if (opening == lastOpening) { return; }
        opening = Mathf.Clamp01(opening);
        for (int i = 0; i < blades.Length; i++) {
            var angle = i * (360f / bladesNumber);
            var rotation = (opening - 0.5f) * (360f / bladesNumber);
            blades[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle + rotation);
        }
        lastOpening = opening;
    }
}
