﻿using UnityEngine;
using System.Collections.Generic;

public class Shutter : MonoBehaviour {

    public const uint MIN_BLADES_NUMBER = 3;

    [RangeAttribute(0f, 1f)]
    public float opening = 1f;
    public uint bladesNumber = 6;
    public float relativeSize = 1f;
    public Material material;

    GameObject[] blades;
    IList<Color> bladeColors = new List<Color>() { Color.black };
    float lastOpening = 1f;
    uint lastBladesNumber = 6;
    float lastRelativeSize = 1f;

    public void SetBladeColors(params Color[] colors) {
        bladeColors = new List<Color>(colors);
        BuildBlades();
    }

    public System.Action<Color> OnColorSelected { get; set; }

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
            AddBladeShape(blade, triangle, ColorForBlade(i));
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

    Color ColorForBlade(uint index) {
        if (bladeColors.Count == 0) { return Color.black; }
        if (bladeColors.Count <= index) { return bladeColors[bladeColors.Count - 1]; }
        return bladeColors[(int)index];
    }

    void AddBladeShape(GameObject blade, Triangle triangle, Color color) {
        var shape = new GameObject("Shape");
        shape.transform.parent = blade.transform;
        shape.transform.localPosition = new Vector2(0f, -triangle.HalfWidth);
        shape.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        shape.AddComponent<MeshFilter>().mesh = triangle.Mesh;
        shape.AddComponent<MeshRenderer>().material = Material(color);
        shape.AddComponent<MeshCollider>();
        shape.AddComponent<ClickListener>().Initialize(OnColorSelected, color);
    }

    Material Material(Color color) {
        var materialCopy = new Material(material);
        materialCopy.color = color;
        return materialCopy;
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

    class ClickListener : MonoBehaviour {

        System.Action<Color> colorSelected;
        Color color;

        public void Initialize(System.Action<Color> colorSelected, Color color) {
            this.colorSelected = colorSelected;
            this.color = color;
        }

        void OnMouseDown() {
            if (colorSelected != null) { colorSelected(color); }
        }
    }
}
