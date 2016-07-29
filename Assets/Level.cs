﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level : MonoBehaviour {

    public Shutter shutter;
    public Qu qu;
    public float closingSpeed;
    public Text scoreboard;
    public Feedback feedback;
    [Tooltip("Color radius decreases like 1/n^difficultyExponent")]
    public float difficultyExponent = 0.6f;
    public float resistance;
    public int duration;

    const float SIZE = 6f;

    RGBColorGenerator colors = new RGBColorGenerator();
    Score scoreAdder = new Score() { basePoints = 100, difficultyMultiplier = 200f };
    Timer timer;
    uint score = 0;
    bool finalClosing = false;
    bool playing;

    void Awake() {
        shutter.relativeSize = SIZE;
        shutter.OnColorSelected = MatchQuColor;
        timer = GetComponent<Timer>();
        closingSpeed = closingSpeed * PlayerPrefs.GetFloat(Preferences.BLADES_SPEED, 1f);
        shutter.bladesNumber = (uint)PlayerPrefs.GetInt(Preferences.BLADES, 3);
        shutter.BackgroundColor = new Color {
            r = PlayerPrefs.GetFloat(Preferences.BACKGROUND_RED, 0.1f),
            g = PlayerPrefs.GetFloat(Preferences.BACKGROUND_GREEN, 0.1f),
            b = PlayerPrefs.GetFloat(Preferences.BACKGROUND_BLUE, 0.1f),
            a = 1f
        };
        shutter.internalCircleRadius = PlayerPrefs.GetFloat(Preferences.INNER_RADIUS, 0.05f);
        resistance = PlayerPrefs.GetFloat(Preferences.RESISTANCE, 1f);
        duration = PlayerPrefs.GetInt(Preferences.DURATION, 60);
    }

    void Start() {
        SetupQuAndBladesColors();
        timer.Set(duration);
        playing = true;
    }

    void MatchQuColor(Color color) {
        if (qu.Dead) { return; }
        if (!playing) { return; }
        if (color == qu.Color) { Succeeded(); } else { Failed(); }
        finalClosing = false;
        Reinitialize();
    }

    void Update() {
        if (finalClosing) { return; }
        if (!playing) { return; }
        if (timer.OutOfTime) {
            playing = false;
            SetMaxScore();
            StartCoroutine(OutOfTimeAnimation());
        } else if (shutter.opening <= 0f) {
            shutter.opening = 0f;
            finalClosing = true;
            StartCoroutine(FinalClosingAnimation());
        } else {
            shutter.opening -= closingSpeed * Time.deltaTime;
        }
    }

    void SetMaxScore() {
        var maxScore = PlayerPrefs.GetInt(Preferences.SCORE, 0);
        if (score > maxScore) {
            PlayerPrefs.SetInt(Preferences.SCORE, (int)score);
            PlayerPrefs.Save();
        }
    }

    void Reinitialize() {
        shutter.ResetOpening();
        qu.Restore();
        SetDifficulty();
        colors.RandomizeCenter();
        SetupQuAndBladesColors();
    }

    void Succeeded() {
        score += scoreAdder.Value;
        scoreAdder.Succeeded();
        scoreboard.text = score.ToString();
        feedback.Ok();
        qu.BeHappy();
    }

    void Failed() {
        scoreAdder.Failed();
        feedback.Wrong();
        qu.BeScared();
    }

    void SetupQuAndBladesColors() {
        var colors = RandomColors();
        var index = Random.Range(0, (int)shutter.bladesNumber);
        shutter.SetBladeColors(colors);
        qu.Color = colors[index];
    }

    Color[] RandomColors() {
        return colors.Generate((int)shutter.bladesNumber);
    }

    void SetDifficulty() {
        var difficulty = Mathf.Pow(scoreAdder.Difficulty, difficultyExponent);
        colors.radius = RGBColorGenerator.MAX_RADIUS / difficulty;
        colors.padding = RGBColorGenerator.MAX_PADDING / difficulty;
    }

    IEnumerator FinalClosingAnimation() {
        if (finalClosing) {
            qu.StretchEyes();
            yield return new WaitForSeconds(resistance);
        }
        if (finalClosing) {
            qu.Die();
            Failed();
        }
        if (finalClosing) {
            yield return new WaitForSeconds(0.5f);
            Reinitialize();
            qu.OpenEyes();
            finalClosing = false;
        }
    }

    public void Quit() {
        SceneManager.LoadScene("Menu");
    }

    IEnumerator OutOfTimeAnimation() {
        var time = 2f;
        while (time > 0) {
            Camera.main.orthographicSize *= 1.1f;
            time -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("Menu");
    }
}
