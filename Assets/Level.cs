﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level : MonoBehaviour {

    const float SIZE = 6f;

    public Shutter shutter;
    public Qu qu;
    public float closingSpeed;
    public Text scoreboard;
    public Feedback feedback;
    public Battery battery;
    [Tooltip("Color radius decreases like 1/n^difficultyExponent")]
    public float difficultyExponent = 1f;
    public float resistance;
    public int duration;
    public AudioClip rightAnswerSound;
    public AudioClip wrongAnswerSound;
    public float PartialStartTime { get { return partialStartTime; } }

    internal Timer timer;

    Score scoreAdder = new Score { basePoints = 10, difficultyMultiplier = 4f, difficultyExponent = 5f };
    RGBColorGenerator colors;
    int score = 0;
    bool finalClosing = false;
    bool playing;

    // Time when level was reinitialized last
    private float partialStartTime;
    private Harvester harvester;

    void Awake() {
        shutter.relativeSize = SIZE;
        shutter.OnColorSelected = MatchQuColor;
        timer = GetComponent<Timer>();
        colors = GetComponent<RGBColorGenerator>();
        closingSpeed = closingSpeed * PlayerPrefs.GetFloat(Preferences.BLADES_SPEED, 1f);
        shutter.bladesNumber = (uint)PlayerPrefs.GetInt(Preferences.BLADES, 3);
        shutter.BackgroundColor = new Color {
            r = PlayerPrefs.GetFloat(Preferences.BACKGROUND_RED, 0.1f),
            g = PlayerPrefs.GetFloat(Preferences.BACKGROUND_GREEN, 0.1f),
            b = PlayerPrefs.GetFloat(Preferences.BACKGROUND_BLUE, 0.1f),
            a = 1f
        };
        shutter.internalCircleRadius = PlayerPrefs.GetFloat(Preferences.INNER_RADIUS, 0.05f);
        resistance = PlayerPrefs.GetFloat(Preferences.RESISTANCE, resistance);
        duration = PlayerPrefs.GetInt(Preferences.DURATION, duration);
        difficultyExponent = PlayerPrefs.GetFloat(Preferences.DIFFICULTY, difficultyExponent);
        harvester = new Harvester();
    }

    void Start() {
        SetupQuAndBladesColors();
        timer.Set(duration);
        playing = true;
        partialStartTime = Time.time;
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
        SetupQuAndBladesColors();
        partialStartTime = Time.time;
    }

    void Succeeded() {
        score += scoreAdder.Value;
        scoreAdder.Succeeded();
        battery.Set(scoreAdder);
        scoreboard.text = score.ToString();
        feedback.Ok();
        qu.BeHappy();
        GetComponent<AudioSource>().PlayOneShot(rightAnswerSound);
        harvester.SaveSingleSessionData(this, succeeded: true);
    }

    void Failed() {
        scoreAdder.Failed();
        battery.Set(scoreAdder);
        feedback.Wrong();
        qu.BeScared();
        GetComponent<AudioSource>().PlayOneShot(wrongAnswerSound);
        harvester.SaveSingleSessionData(this, succeeded: false);
    }

    void SetupQuAndBladesColors() {
        var colors = RandomColors();
        var index = Random.Range(0, (int)shutter.bladesNumber);
        shutter.SetBladeColors(colors);
        qu.Color = colors[index];
    }

    Color[] RandomColors() {
        Color[] randomColors;
        do {
            colors.RandomizeCenter();
            randomColors = colors.Generate((int)shutter.bladesNumber);
        } while (AnyColorIsSimilarToBackground(randomColors));
        return randomColors;
    }

    bool AnyColorIsSimilarToBackground(Color[] colors) {
        const float threshold = 5f / 256f;
        var background = shutter.BackgroundColor;
        foreach (var color in colors) {
            if (Mathf.Abs(background.r - color.r) >= threshold) { continue; }
            if (Mathf.Abs(background.g - color.g) >= threshold) { continue; }
            if (Mathf.Abs(background.b - color.b) >= threshold) { continue; }
            return true;
        }
        return false;
    }

    void SetDifficulty() {
        colors.scale = 1f / Mathf.Pow(scoreAdder.Difficulty, difficultyExponent);
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
        harvester.SendStoredData();
        SceneManager.LoadScene("Menu");
    }

    IEnumerator OutOfTimeAnimation() {
        var time = 2f;
        while (time > 0) {
            Camera.main.orthographicSize *= 1.1f;
            time -= Time.deltaTime;
            yield return null;
        }
        harvester.SendStoredData();
        SceneManager.LoadScene("Menu");
    }
}
