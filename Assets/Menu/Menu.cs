﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Qu qu;
    public Button play;
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
        play.GetComponent<Image>().color = color;
        var meanGrayColor = (color.r + color.g + color.b) / 3f;
        play.GetComponentInChildren<Text>().color = (meanGrayColor < 0.5f) ? Color.white : Color.black;
        title.color = color;
        maxScore.color = color;
    }

    public void StartGame() {
        play.interactable = false;
        GameObject.Find("Buttons").SetActive(false);
        loading.text = "Loading...";
        AudioSource.PlayClipAtPoint(buttonSound, play.transform.position);
        GameManager.Instance.LoadScene(QuScene.MAP);
    }

    public void OpenPreferences() {
        GameManager.Instance.LoadScene(QuScene.SETTINGS);
    }

    public void OpenCards() {
        GameManager.Instance.LoadScene(QuScene.CARD_COLLECTION);
    }

    public void OpenCutscenes() {
       GameManager.Instance.LoadScene(QuScene.CUT_COLLECTION); 
    }

    public void QuitGame() {
        Application.Quit();
    }

    void OnApplicationQuit() {
        GameData.Save();
    }
}
