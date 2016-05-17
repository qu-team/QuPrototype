using UnityEngine;
using System.Collections;

public class Triangle : MonoBehaviour {

    [RangeAttribute(0f, Mathf.PI)]
    public float topAngle = Mathf.PI / 3;
    float oldTopAngle = Mathf.PI / 3;

    public float Width { get { return Mathf.Sin(topAngle / 2) * 2; } }

    public float Height { get { return Mathf.Cos(topAngle / 2); } }

    void Awake() {
        var mesh = gameObject.AddComponent<MeshFilter>().mesh;
        var renderer = gameObject.AddComponent<MeshRenderer>();
        Populate(mesh);
    }

    void Update() {
        if (topAngle == oldTopAngle) { return; }
        topAngle = Mathf.Clamp(topAngle, 0f, Mathf.PI);
        Populate(GetComponent<MeshFilter>().mesh);
        oldTopAngle = topAngle;
    }

    void Populate(Mesh mesh) {
        mesh.Clear();
        var halfWidth = Mathf.Sin(topAngle / 2);
        mesh.vertices = new Vector3[] { new Vector3(0f, Height), new Vector3(halfWidth, 0f), new Vector3(-halfWidth, 0f) };
        mesh.uv = new Vector2[] { new Vector2(0.5f, 1f), new Vector2(1f, 0f), new Vector2() };
        mesh.triangles = new int[] { 0, 1, 2 };
    }
}
