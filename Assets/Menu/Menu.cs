using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Qu qu;
    public GameObject buttons;
    public Text title;
    public Text maxScore;
    public Text loading;
    public AudioClip buttonSound;

    void Start() {
        ColorizeMenuElements(new Color(Random.value, Random.value, Random.value));
        var score = PlayerPrefs.GetInt(Preferences.SCORE, 0);
        if (score > 0) { maxScore.text = string.Format("RECORD: {0}", score); }
        // Send locally cached data to the server
        Harvester.Instance.SendLocalData(this);
    }

    void ColorizeMenuElements(Color color) {
        qu.Color = color;
        ColorizeMenuButtons(buttons, color);
        title.color = color;
        maxScore.color = color;
    }

    void ColorizeMenuButtons(GameObject buttonsParent, Color color) {
        foreach (Transform button in buttonsParent.transform) {
            var image = button.GetComponent<Image>();
            if (image != null) { image.color = color; }
            var meanGrayColor = (color.r + color.g + color.b) / 3f;
            var text = button.GetComponentInChildren<Text>();
            if (text != null) { text.color = (meanGrayColor < 0.5f) ? Color.white : Color.black; }
        }
    }

    public void StartGame() {
        GameObject.Find("Play").GetComponent<Button>().interactable = false;
        buttons.SetActive(false);
        loading.text = L10N.Translate(L10N.Label.LOADING);
        PlayButtonSound();
        GameManager.Instance.LoadScene(QuScene.MAP);
    }

    public void OpenPreferences() {
        PlayButtonSound();
        GameManager.Instance.LoadScene(QuScene.SETTINGS);
    }

    public void OpenCards() {
        PlayButtonSound();
        GameManager.Instance.LoadScene(QuScene.CARD_COLLECTION);
    }

    public void OpenCutscenes() {
        PlayButtonSound();
        GameManager.Instance.LoadScene(QuScene.CUT_COLLECTION);
    }

    void PlayButtonSound() {
        AudioSource.PlayClipAtPoint(buttonSound, buttons.transform.position);
    }

    void OnApplicationQuit() {
        GameData.Save();
    }
}
