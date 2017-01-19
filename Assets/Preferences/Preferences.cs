using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Preferences : MonoBehaviour {

    public const string BLADES_SPEED = "BladesSpeed";
    public const string BACKGROUND_RED = "BackgroundRed";
    public const string BACKGROUND_GREEN = "BackgroundGreen";
    public const string BACKGROUND_BLUE = "BackgroundBlue";
    public const string BLADES = "Blades";
    public const string INNER_RADIUS = "InnerRadius";
    public const string RESISTANCE = "Resistance";
    public const string DURATION = "Duration";
    public const string DIFFICULTY = "Difficulty";
    public const string SCORE = "Score";
    public const string PLAYED_TUTORIAL = "PlayedTutorial";
    public const string LEVEL_UNLOCKED = "LevelUnlocked";

    public Slider bladesSpeedSlider;
    public Text bladesSpeedLabel;
    public Image colorImage;
    public Slider colorRed;
    public Slider colorGreen;
    public Slider colorBlue;
    public Text bladesLabel;
    public Slider bladesSlider;
    public Text innerRadiusLabel;
    public Slider innerRadiusSlider;
    public Text resistanceLabel;
    public Slider resistanceSlider;
    public Text durationLabel;
    public Slider durationSlider;
    public Text difficultyLabel;
    public Slider difficultySlider;

    float bladesSpeed;
    Color color;
    int blades;
    float innerRadius;
    float resistance;
    int duration;
    float difficulty;

    void Awake() {
        bladesSpeedSlider.value = PlayerPrefs.GetFloat(BLADES_SPEED, 1f);
        colorRed.value = PlayerPrefs.GetFloat(BACKGROUND_RED, 0.1f);
        colorGreen.value = PlayerPrefs.GetFloat(BACKGROUND_GREEN, 0.1f);
        colorBlue.value = PlayerPrefs.GetFloat(BACKGROUND_BLUE, 0.1f);
        bladesSlider.value = PlayerPrefs.GetInt(BLADES, 3);
        innerRadiusSlider.value = PlayerPrefs.GetFloat(INNER_RADIUS, 0.049f);
        resistanceSlider.value = PlayerPrefs.GetFloat(RESISTANCE, 1f);
        durationSlider.value = PlayerPrefs.GetInt(DURATION, 60);
        difficultySlider.value = PlayerPrefs.GetFloat(DIFFICULTY, 1f);
        UpdateBladesSpeed();
        UpdateColor();
        UpdateBlades();
        UpdateInnerRadius();
        UpdateResistance();
        UpdateDuration();
        UpdateDifficulty();
    }

    void Update() {
        if (bladesSpeed != bladesSpeedSlider.value) { UpdateBladesSpeed(); }
        if (ColorChanged()) { UpdateColor(); }
        if (blades != bladesSlider.value) { UpdateBlades(); }
        if (innerRadius != innerRadiusSlider.value) { UpdateInnerRadius(); }
        if (resistance != resistanceSlider.value) { UpdateResistance(); }
        if (duration != durationSlider.value) { UpdateDuration(); }
        if (difficulty != difficultySlider.value) { UpdateDifficulty(); }
    }

    void UpdateBladesSpeed() {
        bladesSpeed = bladesSpeedSlider.value;
        bladesSpeedLabel.text = string.Format("Blades speed: {0}%", (int)(bladesSpeed * 100));
    }

    bool ColorChanged() {
        return colorRed.value != color.r || colorGreen.value != color.g || colorBlue.value != color.b;
    }

    void UpdateColor() {
        color = new Color(colorRed.value, colorGreen.value, colorBlue.value);
        colorImage.color = color;
    }

    void UpdateBlades() {
        blades = (int)bladesSlider.value;
        bladesLabel.text = "Blades: " + blades;
    }

    void UpdateInnerRadius() {
        innerRadius = innerRadiusSlider.value;
        innerRadiusLabel.text = string.Format("Inner radius: {0}", innerRadius.ToString("0.00"));
    }

    void UpdateResistance() {
        resistance = resistanceSlider.value;
        resistanceLabel.text = string.Format("Resistance: {0}s", resistance.ToString("0.0"));
    }

    void UpdateDuration() {
        duration = (int)durationSlider.value;
        durationLabel.text = string.Format("Duration: {0}s", duration);
    }

    void UpdateDifficulty() {
        difficulty = difficultySlider.value;
        difficultyLabel.text = string.Format("Difficulty: 1 / n^{0}", difficulty.ToString("0.0"));
    }

    public void Cancel() {
        SceneManager.LoadScene("Menu");
    }

    public void Save() {
        PlayerPrefs.SetFloat(BLADES_SPEED, bladesSpeed);
        PlayerPrefs.SetFloat(BACKGROUND_RED, color.r);
        PlayerPrefs.SetFloat(BACKGROUND_GREEN, color.g);
        PlayerPrefs.SetFloat(BACKGROUND_BLUE, color.b);
        PlayerPrefs.SetInt(BLADES, blades);
        PlayerPrefs.SetFloat(INNER_RADIUS, innerRadius);
        PlayerPrefs.SetFloat(RESISTANCE, resistance);
        PlayerPrefs.SetInt(DURATION, duration);
        PlayerPrefs.SetFloat(DIFFICULTY, difficulty);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }

    public void Reset() {
        bladesSpeedSlider.value = 1f;
        colorRed.value = 0.1f;
        colorGreen.value = 0.1f;
        colorBlue.value = 0.1f;
        bladesSlider.value = 3;
        innerRadiusSlider.value = 0.049f;
        resistanceSlider.value = 1f;
        durationSlider.value = 60;
        difficultySlider.value = 1f;
        UpdateBladesSpeed();
        UpdateColor();
        UpdateBlades();
        UpdateInnerRadius();
        UpdateResistance();
        UpdateDuration();
        UpdateDifficulty();
    }

    public void ResetMaxScore() {
        PlayerPrefs.SetInt(SCORE, 0);
        PlayerPrefs.Save();
    }
}
