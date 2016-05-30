﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThreeBladesLevel : MonoBehaviour {

    public Shutter shutter;
    public Qu qu;
    public float closingSpeed;
    public Text scoreboard;

    const float SIZE = 6f;
    const float MAX_OPENING = 0.25f;
    const float MIN_OPENING = 0.04f;

    ColorGenerator colors = new RGBColorGenerator();
    Score scoreAdder = new Score() { basePoints = 10, difficultyMultiplier = 2f, comboMultiplier = 3f };
    uint score = 0;
    bool finalClosing = false;

    void Awake() {
        shutter.bladesNumber = 3;
        shutter.relativeSize = SIZE;
        shutter.opening = MAX_OPENING;
    }

    void Start() {
        SetupQuAndBladesColors();
    }

    void Update() {
        if (finalClosing) { return; }
        if (shutter.opening == 0f) {
            StubGameplay();
            shutter.opening = MAX_OPENING;
            qu.Restore();
            SetDifficulty();
            RandomizeColorSpace();
            SetupQuAndBladesColors();
        } else if (shutter.opening <= MIN_OPENING) {
            finalClosing = true;
            StartCoroutine(FinalClosingAnimation());
        } else {
            shutter.opening -= closingSpeed * Time.deltaTime;
        }
    }

    void StubGameplay() {
        if (Random.value <= 0.75f) {
            score += scoreAdder.Value;
            scoreAdder.Succeeded();
            scoreboard.text = score.ToString();
        } else {
            scoreAdder.Failed();
        }
    }

    void SetupQuAndBladesColors() {
        var colors = new Color[] { RandomColor, RandomColor, RandomColor };
        var index = Random.Range(0, 2);
        shutter.SetBladeColors(colors);
        qu.SetColor(colors[index]);
    }

    void RandomizeColorSpace() {
        var radius = colors.MaxRadius - colors.Radius;
        var x = Random.Range(-1f, 1f);
        var y = Random.Range(-1f, 1f);
        var z = Random.Range(-1f, 1f);
        colors.Position = new Vector3(x, y, z) * radius + colors.Center;
    }

    void SetDifficulty() {
        colors.Radius = colors.MaxRadius / scoreAdder.Difficulty;
    }

    Color RandomColor { get { return colors.Generate(); } }

    IEnumerator FinalClosingAnimation() {
        qu.StretchEyes();
        yield return new WaitForSeconds(0.5f);
        qu.Die();
        while (shutter.opening >= 0.0001f) {
            shutter.opening /= 1.1f;
            yield return null;
        }
        shutter.opening = 0f;
        finalClosing = false;
        yield return new WaitForSeconds(0.5f);
    }
}
