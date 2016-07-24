using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Qu qu;
    public Button play;
    public Text title;
    public Text maxScore;
    public AudioClip buttonSound;

    void Start() {
        ColorizeMenuElements(new Color(Random.value, Random.value, Random.value));
    }

    void ColorizeMenuElements(Color color) {
        qu.Color = color;
        play.GetComponent<Image>().color = color;
        var meanGrayColor = (color.r + color.g + color.b) / 3f;
        play.GetComponentInChildren<Text>().color = (meanGrayColor < 0.5f) ? Color.white : Color.black;
        title.color = color;
        maxScore.color = color;
    }

    public void StartGame() {
        play.interactable = false;
        AudioSource.PlayClipAtPoint(buttonSound, play.transform.position);
        SceneManager.LoadSceneAsync("Level");
    }

    public void OpenPreferences() {
        SceneManager.LoadScene("Preferences");
    }
}
