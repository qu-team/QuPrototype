using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Qu qu;
    public Button button;
    public Text title;
    public AudioClip buttonSound;

    void Start() {
        ColorizeMenuElements(new Color(Random.value, Random.value, Random.value));
    }

    void ColorizeMenuElements(Color color) {
        qu.Color = color;
        button.GetComponent<Image>().color = color;
        var meanGrayColor = (color.r + color.g + color.b) / 3f;
        button.GetComponentInChildren<Text>().color = (meanGrayColor < 0.5f) ? Color.white : Color.black;
        title.color = color;
    }

    public void StartGame() {
        button.interactable = false;
        AudioSource.PlayClipAtPoint(buttonSound, button.transform.position);
        SceneManager.LoadSceneAsync("Level");
    }
}
