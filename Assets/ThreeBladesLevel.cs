using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ThreeBladesLevel : MonoBehaviour {

    public Shutter shutter;
    public Qu qu;
    public float closingSpeed;
    public Text scoreboard;
    public Feedback feedback;
    [Tooltip("Color radius decreases like 1/n^difficultyExponent")]
    public float difficultyExponent = 0.6f;

    const float SIZE = 6f;
    const float MAX_OPENING = 0.25f;
    const float MIN_OPENING = 0.04f;

    ColorGenerator colors = new RGBColorGenerator();
    Score scoreAdder = new Score() { basePoints = 100, difficultyMultiplier = 50f, comboMultiplier = 100f };
    Timer timer;
    uint score = 0;
    bool finalClosing = false;

    void Awake() {
        shutter.bladesNumber = 3;
        shutter.relativeSize = SIZE;
        shutter.opening = MAX_OPENING;
        shutter.OnColorSelected = MatchQuColor;
        timer = GetComponent<Timer>();
    }

    void Start() {
        SetupQuAndBladesColors();
        timer.Restart(60f);
    }

    void MatchQuColor(Color color) {
        if (qu.Dead) { return; }
        if (color == qu.Color) { Succeeded(); } else { Failed(); }
        finalClosing = false;
        Reinitialize();
    }

    void Update() {
        if (finalClosing) { return; }
        if (timer.OutOfTime) {
            score = 0;
            scoreboard.text = score.ToString();
            SceneManager.LoadScene("Menu");
        } else if (shutter.opening == 0f) {
            Reinitialize();
        } else if (shutter.opening <= MIN_OPENING) {
            finalClosing = true;
            StartCoroutine(FinalClosingAnimation());
        } else {
            shutter.opening -= closingSpeed * Time.deltaTime;
        }
    }

    void Reinitialize() {
        shutter.opening = MAX_OPENING;
        qu.Restore();
        SetDifficulty();
        RandomizeColorSpace();
        SetupQuAndBladesColors();
    }

    void Succeeded() {
        score += scoreAdder.Value;
        if (score > GameData.MaxScore) { GameData.MaxScore = score; }
        scoreAdder.Succeeded();
        scoreboard.text = score.ToString();
        feedback.Ok();
    }

    void Failed() {
        scoreAdder.Failed();
        feedback.Wrong();
    }

    void SetupQuAndBladesColors() {
        var colors = RandomColors();
        var index = Random.Range(0, (int)shutter.bladesNumber);
        shutter.SetBladeColors(colors);
        qu.Color = colors[index];
    }

    Color[] RandomColors() {
        var colors = new Color[shutter.bladesNumber];
        for (int i = 0; i < colors.Length; i++) { colors[i] = RandomColor; }
        return colors;
    }

    void RandomizeColorSpace() {
        var radius = colors.MaxRadius - colors.Radius;
        var x = Random.Range(-1f, 1f);
        var y = Random.Range(-1f, 1f);
        var z = Random.Range(-1f, 1f);
        colors.Position = new Vector3(x, y, z) * radius + colors.Center;
    }

    void SetDifficulty() {
        colors.Radius = colors.MaxRadius / Mathf.Pow(scoreAdder.Difficulty, difficultyExponent);
	colors.MinRadius = colors.InitialMinRadius / Mathf.Pow(scoreAdder.Difficulty, difficultyExponent);
    }

    Color RandomColor { get { return colors.Generate(); } }

    IEnumerator FinalClosingAnimation() {
        if (finalClosing) {
            qu.StretchEyes();
            yield return new WaitForSeconds(0.5f);
        }
        if (finalClosing) {
            qu.Die();
            Failed();
        }
        while (finalClosing && shutter.opening >= 0.0001f) {
            shutter.opening /= 1.1f;
            yield return null;
        }
        if (finalClosing) {
            shutter.opening = 0f;
            finalClosing = false;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
