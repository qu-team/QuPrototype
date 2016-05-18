using UnityEngine;

public struct Triangle {

    public float topAngle;
    public float relativeSize;

    public float HalfWidth { get { return Mathf.Sin(topAngle / 2) * relativeSize; } }

    public float Height { get { return Mathf.Cos(topAngle / 2) * relativeSize; } }

    public Mesh Mesh {
        get {
            var mesh = new Mesh();
            mesh.vertices = new Vector3[] { new Vector3(0f, Height), new Vector3(HalfWidth, 0f), new Vector3(-HalfWidth, 0f) };
            mesh.uv = new Vector2[] { new Vector2(0.5f, 1f), new Vector2(1f, 0f), new Vector2() };
            mesh.triangles = new int[] { 0, 1, 2 };
            return mesh;
        }
    }
}
