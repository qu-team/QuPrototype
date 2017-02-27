﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    const float SIZE = 6f;

    public Shutter shutter;
    public Qu qu;
    public float closingSpeed;
    public Text scoreboard;
    public Feedback feedback;
    public Battery battery;
    public GameObject stars;
    [Tooltip("Color radius decreases like 1/n^difficultyExponent")]
    public float difficultyExponent = 1f;
    public float resistance;
    public uint duration;
    public AudioClip rightAnswerSound;
    public AudioClip wrongAnswerSound;
    public float PartialStartTime { get { return partialStartTime; } }

    public bool IsTutorial {
        get;
        internal set;
    }

    internal Timer timer;

    Score scoreAdder = new Score { basePoints = 10, difficultyMultiplier = 4f };
    IColorGenerator colors;
    int score = 0;
    bool finalClosing = false;
    bool playing;
    bool maxScoreReached = false;
    // Time when level was reinitialized last
    float partialStartTime;
    Harvester harvester;
    // Keeps track of the number of saved qu, max score, etc (for this session)
    LevelSaveData levelData;

    void Awake() {
        shutter.relativeSize = SIZE;
        shutter.OnColorSelected = MatchQuColor;
        timer = GetComponent<Timer>();
        harvester = Harvester.Instance;
        levelData = new LevelSaveData();
        GameManager.Instance.LevelLoaded();
    }

    void LoadLevelPrefs() {
        var gm = GameManager.Instance;
        var level = gm.Levels[gm.CurrentLevel];
        closingSpeed = closingSpeed * level.bladesSpeed;
        shutter.bladesNumber = level.blades;
        shutter.BackgroundColor = level.bgColor;
        shutter.internalCircleRadius = level.innerRadius;
        resistance = level.quResistance;
        duration = level.duration;
        difficultyExponent = level.difficultyExp;
        CreateColorGenerator();
    }

    void CreateColorGenerator() {
        var gm = GameManager.Instance;
        var level = gm.Levels[gm.CurrentLevel];
        // Create the ColorGenerator
        colors = new HSLColorGenerator();
        if (level.saturation > 0)
            (colors as HSLColorGenerator).saturation = level.saturation;
        if (level.brightness > 0)
            (colors as HSLColorGenerator).lightness = level.brightness;
        //if (level.saturation > 0 || level.brightness > 0) {
            //if (level.saturation > 0)
                //(colors as HSLColorGenerator).saturation = level.saturation;
            //if (level.brightness > 0)
                //(colors as HSLColorGenerator).lightness = level.brightness;
        //} else {
            //// default
            //colors = new LABColorGenerator();
        //}
    }

    void Start() {
        if (IsTutorial) {
            CreateColorGenerator();
            SetTutorialSettings();
        } else {
            LoadLevelPrefs();
            DisableTutorialGraphics();
        }
        SetupQuAndBladesColors();
        timer.Set(duration);
        playing = true;
        partialStartTime = Time.time;
    }

    void SetTutorialSettings() {
        shutter.bladesNumber = 3;
        shutter.BackgroundColor = new Color(45 / 255f, 45 / 255f, 45 / 255f);
        duration = 10;
        GameObject.Find("Score").GetComponent<Text>().text = "TUTORIAL";
    }

    void DisableTutorialGraphics() {
        GameObject.Find("Hand").SetActive(false);
        GameObject.Find("Arrow").SetActive(false);
    }

    internal void MatchQuColor(Color color) {
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
            StartCoroutine(OutOfTimeAnimation());
        } else if (shutter.opening <= 0f) {
            shutter.opening = 0f;
            finalClosing = true;
            StartCoroutine(FinalClosingAnimation());
        } else if (timer.enabled) {
            shutter.opening -= closingSpeed * Time.deltaTime;
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
        levelData.quSaved++;
        levelData.maxCombo = (uint)Mathf.Max(levelData.maxCombo, scoreAdder.Combo);
        int lv = GameManager.Instance.CurrentLevel;
        // Check if we reached the max score
        if (score > levelData.maxScore) {
            levelData.maxScore = score;
            Debug.Assert(GameData.data.levels != null 
                    && GameData.data.levels.Count > GameManager.Instance.CurrentLevel);
            if (score > GameData.data.levels[GameManager.Instance.CurrentLevel].maxScore)
                maxScoreReached = true;
        }
    }

    void Failed() {
        scoreAdder.Failed();
        battery.Set(scoreAdder);
        feedback.Wrong();
        qu.BeScared();
        timer.TimePenality(SecondsOfPenality);
        GetComponent<AudioSource>().PlayOneShot(wrongAnswerSound);
        harvester.SaveSingleSessionData(this, succeeded: false);
    }

    float SecondsOfPenality { get { return 5f - (Time.time - partialStartTime); } }

    void SetupQuAndBladesColors() {
        var rcolors = RandomColors();
        var index = Random.Range(0, (int)shutter.bladesNumber);
        shutter.SetBladeColors(rcolors);
        qu.Color = rcolors[index];
    }

    Color[] RandomColors() {
        Color[] randomColors;
        do {
            randomColors = colors.Generate((int)shutter.bladesNumber);
        } while (AnyColorIsSimilarToBackground(randomColors));
        return randomColors;
    }

    bool AnyColorIsSimilarToBackground(Color[] cols) {
        const float threshold = 5f / 256f;
        var background = shutter.BackgroundColor;
        foreach (var color in cols) {
            if (Mathf.Abs(background.r - color.r) >= threshold) { continue; }
            if (Mathf.Abs(background.g - color.g) >= threshold) { continue; }
            if (Mathf.Abs(background.b - color.b) >= threshold) { continue; }
            return true;
        }
        return false;
    }

    void SetDifficulty() {
        //colors.Difficulty = Mathf.Pow(scoreAdder.Difficulty, difficultyExponent);
        colors.Difficulty = scoreAdder.Difficulty * difficultyExponent;
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
            qu.StopDying();
            qu.OpenEyes();
            finalClosing = false;
        }
    }

    public void Pause() {
        timer.enabled = false;
    }

    public void Resume() {
        timer.enabled = true;
    }

    public bool Paused { get { return !timer.enabled; } }

    public void Quit() {
        GameManager.Instance.Back();
    }

    IEnumerator OutOfTimeAnimation() {
        var time = 2f;
        EnableStars();
        while (time > 0) {
            Camera.main.orthographicSize *= 1.1f;
            SetStarsAlpha(Mathf.Min(time, 2f - time));
            time -= Time.deltaTime;
            yield return null;
        }
        SetStarsAlpha(0f);
        if (!IsTutorial) {
            LogHelper.Info(this, "Saving data");
            SaveData();
        } else {
            LogHelper.Info(this, "Level is tutorial: not saving data");
            PlayerPrefs.SetInt(Preferences.PLAYED_TUTORIAL, 1);
        }
        LoadNextScene();
    }

    void EnableStars() {
        stars.SetActive(true);
        var starScores = GameManager.Instance.Levels[GameManager.Instance.CurrentLevel].stars;
        if (starScores.first > score) { stars.transform.FindChild("Star1").gameObject.SetActive(false); }
        if (starScores.second > score) { stars.transform.FindChild("Star2").gameObject.SetActive(false); }
        if (starScores.third > score) { stars.transform.FindChild("Star3").gameObject.SetActive(false); }
    }

    void SetStarsAlpha(float alpha) {
        foreach (Transform star in stars.transform) {
            var image = star.gameObject.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }

    void SaveData() {
        harvester.SendStoredData(this);
        var game = GameManager.Instance;
        var lv = game.CurrentLevel;
        GameData.data.levels[lv] = GameData.data.levels[lv].Overwrite(levelData);
        // Check if we unlocked next level
        if (lv == GameData.data.curLevelUnlocked && GameData.data.levels[lv].quSaved >= game.Levels[lv].quToNextLevel) {
            ++GameData.data.curLevelUnlocked;
        }
        GameData.Save();
    }

    void LoadNextScene() {
        var nextScene = (!IsTutorial && maxScoreReached) ? QuScene.SHARE : QuScene.MAP;
        GameManager.Instance.ShowUnlockedCardsThenGoTo(nextScene);
    }
}
