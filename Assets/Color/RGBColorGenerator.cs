using UnityEngine;

public class RGBColorGenerator : ColorGenerator {

    Vector3 position = new Vector3(0.5f, 0.5f, 0.5f);
    float radius = 0.5f;

    public float MaxRadius { get { return 0.5f; } }

    public Vector3 Center { get { return new Vector3(0.5f, 0.5f, 0.5f); } }

    public Vector3 Position {
        get { return position; }
        set { position = new Vector3(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y), Mathf.Clamp01(value.z)); }
    }

    public float Radius {
        get { return radius; }
        set { radius = Mathf.Clamp01(value); }
    }

    public Color Generate() {
        var r = Random.Range(-1f, 1f) * radius + position.x;
        var g = Random.Range(-1f, 1f) * radius + position.y;
        var b = Random.Range(-1f, 1f) * radius + position.z;
        return new Color(r, g, b);
    }
}
