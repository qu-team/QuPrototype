using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ShareScore : MonoBehaviour {

    public Text scoreLabel;

    int score;

    void Awake() {
        score = PlayerPrefs.GetInt(Preferences.SCORE, 0);
        scoreLabel.text = score.ToString();
    }

    public void ShareOnFacebook() {
        string shareUrl = "https://www.facebook.com/sharer/sharer.php";
        shareUrl += "?title=" + Uri.EscapeUriString("New high score on qU!");
        shareUrl += "&description=" + Uri.EscapeUriString("I scored " + score + " points!");
        shareUrl += "&u=" + Uri.EscapeUriString("https://github.com/qu-team/QuPrototype");
        Application.OpenURL(shareUrl);
    }

    public void ShareOnTwitter() {
        string shareUrl = "https://twitter.com/intent/tweet?text=" + Uri.EscapeUriString("I scored " + score + " points on qU!");
        Application.OpenURL(shareUrl);
    }

    public void Continue() {
        SceneManager.LoadScene("Menu");
    }
}
