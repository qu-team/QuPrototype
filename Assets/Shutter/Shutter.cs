using UnityEngine;

public class Shutter : MonoBehaviour {
    
    public Sprite bladeImage;
    [RangeAttribute(0f, 1f)]
    public float opening = 1f;

    GameObject[] blades = new GameObject[6];
    float lastOpening = 1f;

    void Awake() {
        var portion = Mathf.PI / 3;
        for (int i = 0; i < blades.Length; i++) {
            var angle = i * 60f + 30f;
            var position = new Vector2(Mathf.Cos(i * portion), Mathf.Sin(i * portion));
            blades[i] = new GameObject("Blade" + i);
            blades[i].transform.parent = transform;
            blades[i].transform.localPosition = position;
            blades[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle);
            var sprite = new GameObject("Sprite");
            sprite.transform.parent = blades[i].transform;
            sprite.transform.localPosition = new Vector2(-0.5f, -0.5f);
            sprite.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            var spriteRenderer = sprite.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = bladeImage;
        }
    }

    void Update() {
        if (opening == lastOpening) { return; }
        opening = Mathf.Clamp01(opening);
        for (int i = 0; i < blades.Length; i++) {
            var angle = i * 60f + 30f;
            var rotation = (opening - 1) * 60f;
            blades[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle + rotation);
        }
        lastOpening = opening;
    }
}
