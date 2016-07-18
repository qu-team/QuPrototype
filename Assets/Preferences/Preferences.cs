using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Preferences : MonoBehaviour {

    public static readonly string BLADES_SPEED = "BladesSpeed";
    public static readonly string BACKGROUND_RED = "BackgroundRed";
    public static readonly string BACKGROUND_GREEN = "BackgroundGreen";
    public static readonly string BACKGROUND_BLUE = "BackgroundBlue";
    public static readonly string BLADES = "Blades";

    public Slider bladesSpeedSlider;
    public Text bladesSpeedLabel;
    public Image colorImage;
    public Slider colorRed;
    public Slider colorGreen;
    public Slider colorBlue;
    public Text bladesLabel;
    public Slider bladesSlider;

    float bladesSpeed;
    Color color;
    int blades;

    void Awake() {
        bladesSpeedSlider.value = PlayerPrefs.GetFloat(BLADES_SPEED, 1f);
        colorRed.value = PlayerPrefs.GetFloat(BACKGROUND_RED, 0.1f);
        colorGreen.value = PlayerPrefs.GetFloat(BACKGROUND_GREEN, 0.1f);
        colorBlue.value = PlayerPrefs.GetFloat(BACKGROUND_BLUE, 0.1f);
        bladesSlider.value = PlayerPrefs.GetInt(BLADES, 3);
    }

    void Update() {
        if (bladesSpeed != bladesSpeedSlider.value) {
            bladesSpeed = bladesSpeedSlider.value;
            bladesSpeedLabel.text = string.Format("Blades speed: {0:#.#}", bladesSpeed);
        }
        if (colorRed.value != color.r || colorGreen.value != color.g || colorBlue.value != color.b) {
            color = new Color(colorRed.value, colorGreen.value, colorBlue.value);
            colorImage.color = color;
        }
        if (blades != bladesSlider.value) {
            blades = (int)bladesSlider.value;
            bladesLabel.text = "Blades: " + blades;
        }
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
        PlayerPrefs.Save();
        SceneManager.LoadScene("Menu");
    }
}
