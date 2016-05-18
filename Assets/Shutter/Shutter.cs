using UnityEngine;

public class Shutter : MonoBehaviour {

    public const uint MIN_BLADES_NUMBER = 3;

    [RangeAttribute(0f, 1f)]
    public float opening = 1f;
    public uint bladesNumber = 6;
    public float relativeSize = 1f;

    GameObject[] blades;
    float lastOpening = 1f;
    uint lastBladesNumber = 6;
    float lastRelativeSize = 1f;

    void Awake() {
        BuildBlades();
    }

    void BuildBlades() {
        DestroyPreviousBlades();
        blades = new GameObject[bladesNumber];
        var triangle = new Triangle() { topAngle = Mathf.PI * 2 / bladesNumber, relativeSize = relativeSize };
        for (uint i = 0; i < bladesNumber; i++) {
            var blade = new GameObject("Blade" + i);
            blade.transform.parent = transform;
            blade.transform.localPosition = new Vector2(Mathf.Cos(i * triangle.topAngle), Mathf.Sin(i * triangle.topAngle)) * relativeSize;
            AddBladeShape(blade, triangle);
            blades[i] = blade;
        }
        UpdateBladesRotation();
    }

    void DestroyPreviousBlades() {
        if (blades != null) {
            foreach (var blade in blades) { GameObject.Destroy(blade); }
            blades = null;
        }
    }

    void AddBladeShape(GameObject blade, Triangle triangle) {
        var shape = new GameObject("Shape");
        shape.transform.parent = blade.transform;
        shape.transform.localPosition = new Vector2(0f, -triangle.HalfWidth);
        shape.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        shape.AddComponent<MeshFilter>().mesh = triangle.Mesh;
        shape.AddComponent<MeshRenderer>();
    }

    void UpdateBladesRotation() {
        for (uint i = 0; i < bladesNumber; i++) {
            var angle = i * (360f / bladesNumber);
            var rotation = (opening - 0.5f) * (360f / bladesNumber);
            blades[i].transform.localRotation = Quaternion.Euler(0f, 0f, angle + rotation);
        }
    }

    void Update() {
        if (opening != lastOpening) {
            opening = Mathf.Clamp01(opening);
            UpdateBladesRotation();
            lastOpening = opening;
        }
        if (bladesNumber != lastBladesNumber || relativeSize != lastRelativeSize) {
            if (bladesNumber < MIN_BLADES_NUMBER) { bladesNumber = MIN_BLADES_NUMBER; }
            if (relativeSize < 0f) { relativeSize = 0f; }
            BuildBlades();
            lastBladesNumber = bladesNumber;
            lastRelativeSize = relativeSize;
        }
    }
}
