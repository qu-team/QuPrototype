using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ShareScore : MonoBehaviour {

    public Text scoreLabel;

    long score;

    void Awake() {
        score = GameData.data.levels[GameManager.Instance.CurrentLevel].maxScore;
        scoreLabel.text = score.ToString();
    }

    public void ShareOnFacebook() {
        string shareUrl = "https://www.facebook.com/sharer/sharer.php";
        shareUrl += "?title=" + Uri.EscapeUriString(L10N.Translate(L10N.Label.FB_NEW_HIGH_SCORE));
        shareUrl += "&description=" + Uri.EscapeUriString(L10N.Translate(L10N.Label.FB_SCORED, score));
        shareUrl += "&u=" + Uri.EscapeUriString("https://github.com/qu-team/QuPrototype");
        Application.OpenURL(shareUrl);
    }

    public void ShareOnTwitter() {
        string shareUrl = "https://twitter.com/intent/tweet?text=";
        shareUrl += Uri.EscapeUriString(L10N.Translate(L10N.Label.TWIT, score));
        Application.OpenURL(shareUrl);
    }

    public void Continue() {
        GameManager.Instance.LoadScene(QuScene.MAP);
    }
}
