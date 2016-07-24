using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Preferences : MonoBehaviour {

    public static readonly string BLADES_SPEED = "BladesSpeed";
    public static readonly string BACKGROUND_RED = "BackgroundRed";
    public static readonly string BACKGROUND_GREEN = "BackgroundGreen";
    public static readonly string BACKGROUND_BLUE = "BackgroundBlue";
    public static readonly string BLADES = "Blades";
    public static readonly string FINAL_APERTURE = "FinalAperture";

    public Slider bladesSpeedSlider;
    public Text bladesSpeedLabel;
    public Image colorImage;
    public Slider colorRed;
    public Slider colorGreen;
    public Slider colorBlue;
    public Text bladesLabel;
    public Slider bladesSlider;
    public Text finalApertureLabel;
    public Slider finalApertureSlider;

    float bladesSpeed;
    Color color;
    int blades;
    float finalAperture;

    void Awake() {
        bladesSpeedSlider.value = PlayerPrefs.GetFloat(BLADES_SPEED, 1f);
        colorRed.value = PlayerPrefs.GetFloat(BACKGROUND_RED, 0.1f);
        colorGreen.value = PlayerPrefs.GetFloat(BACKGROUND_GREEN, 0.1f);
        colorBlue.value = PlayerPrefs.GetFloat(BACKGROUND_BLUE, 0.1f);
        bladesSlider.value = PlayerPrefs.GetInt(BLADES, 3);
        finalApertureSlider.value = PlayerPrefs.GetFloat(FINAL_APERTURE, 0f);
        UpdateBladesSpeed();
        UpdateColor();
        UpdateBlades();
        UpdateFinalAperture();
    }

    void Update() {
        if (bladesSpeed != bladesSpeedSlider.value) { UpdateBladesSpeed(); }
        if (ColorChanged()) { UpdateColor(); }
        if (blades != bladesSlider.value) { UpdateBlades(); }
        if (finalAperture != finalApertureSlider.value) { UpdateFinalAperture(); }
    }

    void UpdateBladesSpeed() {
        bladesSpeed = bladesSpeedSlider.value;
        bladesSpeedLabel.text = string.Format("Blades speed: {0}%", (int) (bladesSpeed * 100));
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

    void UpdateFinalAperture() {
        finalAperture = finalApertureSlider.value;
        finalApertureLabel.text = string.Format("Final aperture: {0}%", (int)(finalAperture * 100));
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
        PlayerPrefs.SetFloat(FINAL_APERTURE, finalAperture);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
}
