using UnityEngine;
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
    [Tooltip("Color radius decreases like 1/n^difficultyExponent")]
    public float difficultyExponent = 1f;
    public float resistance;
    public uint duration;
    public AudioClip rightAnswerSound;
    public AudioClip wrongAnswerSound;
    public float PartialStartTime { get { return partialStartTime; } }

    internal Timer timer;

    Score scoreAdder = new Score { basePoints = 10, difficultyMultiplier = 4f, difficultyExponent = 5f };
    //RGBColorGenerator colors;
    HSLColorGenerator colors;
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
        colors = GetComponent<HSLColorGenerator>();
        LoadLevelPrefs();
        harvester = Harvester.Instance;
        levelData = new LevelSaveData();
    }

    void LoadLevelPrefs() {
        var gm = GameManager.Instance;
        var level = gm.Levels[gm.CurrentLevel];
        closingSpeed = closingSpeed * level.bladesSpeed;
        shutter.bladesNumber = level.blades;
        shutter.BackgroundColor = new Color {
            r = level.bgColor.r,
            g = level.bgColor.g,
            b = level.bgColor.b,
            a = 1f
        };
        shutter.internalCircleRadius = level.innerRadius;
        resistance = PlayerPrefs.GetFloat(Preferences.RESISTANCE, resistance);
        duration = level.duration;
        difficultyExponent = level.difficultyExp;
        //closingSpeed = closingSpeed * PlayerPrefs.GetFloat(Preferences.BLADES_SPEED, 1f);
        //shutter.bladesNumber = (uint)PlayerPrefs.GetInt(Preferences.BLADES, 3);
        //shutter.BackgroundColor = new Color {
            //r = PlayerPrefs.GetFloat(Preferences.BACKGROUND_RED, 0.1f),
            //g = PlayerPrefs.GetFloat(Preferences.BACKGROUND_GREEN, 0.1f),
            //b = PlayerPrefs.GetFloat(Preferences.BACKGROUND_BLUE, 0.1f),
            //a = 1f
        //};
        //shutter.internalCircleRadius = PlayerPrefs.GetFloat(Preferences.INNER_RADIUS, 0.05f);
        //resistance = PlayerPrefs.GetFloat(Preferences.RESISTANCE, resistance);
        //duration = PlayerPrefs.GetInt(Preferences.DURATION, duration);
        //difficultyExponent = PlayerPrefs.GetFloat(Preferences.DIFFICULTY, difficultyExponent);
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
            StartCoroutine(OutOfTimeAnimation());
        } else if (shutter.opening <= 0f) {
            shutter.opening = 0f;
            finalClosing = true;
            StartCoroutine(FinalClosingAnimation());
        } else {
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
        int lv = GameManager.Instance.CurrentLevel;
        // Check if we reached the max score
        if (score > levelData.maxScore) {
            levelData.maxScore = score;
            if (GameData.data.levels == null
                            || GameData.data.levels.Count <= GameManager.Instance.CurrentLevel
                            || score > GameData.data.levels[GameManager.Instance.CurrentLevel].maxScore)
            {
                maxScoreReached = true;
            }
        }
        // Check if we saved enough Qu for next level
        if (levelData.quSaved == GameManager.Instance.Levels[lv].quToNextLevel) {
            GameData.data.curLevelUnlocked++;
            // TODO: show text "unlocked level"
        }
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
            //colors.RandomizeCenter();
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
        //colors.scale = 1f / Mathf.Pow(scoreAdder.Difficulty, difficultyExponent);
        colors.arcAmplitude = 360f / (scoreAdder.Difficulty * difficultyExponent);
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
        SaveData();
        LoadNextScene();
    }

    void SaveData() {
        harvester.SendStoredData(this);
        var gm = GameManager.Instance;
        var lv = gm.CurrentLevel;
        print("lv is " + gm.CurrentLevel);
        EnsureLevelsAreInitialized(gm);
        GameData.data.levels[lv] = GameData.data.levels[lv].Overwrite(levelData);
        GameData.Save();
    }

    void EnsureLevelsAreInitialized(GameManager gm) {
        if (GameData.data.levels == null) { GameData.data.levels = new List<LevelSaveData>(); }
        if (GameData.data.levels.Count == 0) {
            print("Initializing data for " + gm.Levels.Count + " levels");
            for (int i = 0; i < gm.Levels.Count; ++i) {
                GameData.data.levels.Add(new LevelSaveData());
            }
        }
    }

    void LoadNextScene() {
        SceneManager.LoadScene(maxScoreReached ? "ShareScore" : "MapScene");
    }
}
