using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Qu qu;

    void Start() {
        qu.Color = new Color(Random.value, Random.value, Random.value);
    }

    public void StartGame() {
        SceneManager.LoadScene("Level");
    }
}
