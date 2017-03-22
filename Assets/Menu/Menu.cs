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
        Screen.fullScreen = false;
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
            //var meanGrayColor = (color.r + color.g + color.b) / 3f;
        }
    }

    public void StartGame() {
        GameObject.Find("Play").GetComponent<Button>().interactable = false;
        buttons.SetActive(false);
        loading.text = L10N.Translate(L10N.Label.LOADING);
        PlayButtonSound();
        Screen.fullScreen = true;
#if !UNITY_EDITOR
        // If tutorial was not played yet, play it
        if (PlayerPrefs.GetInt(Preferences.PLAYED_TUTORIAL, 0) == 0) {
            GameManager.Instance.LoadScene(QuScene.TUTORIAL);
        } else {
            GameManager.Instance.LoadScene(QuScene.MAP);
        }
#else
            GameManager.Instance.LoadScene(QuScene.MAP);
#endif
    }

    public void OpenSettings() {
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
}
