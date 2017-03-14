using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour {

    RectTransform fill;
    Image fillImage;
    Text percentage;
    int combo = 0;

    void Awake() {
        fill = transform.GetChild(0).GetComponent<RectTransform>();
        fillImage = transform.GetChild(0).GetComponent<Image>();
        percentage = transform.GetChild(1).GetComponent<Text>();
        fill.localScale = new Vector3(0f, 1f, 1f);
        fillImage.color = Color.red;
        percentage.text = string.Format("x0");
    }

    //void Update() {
        //float a = combo / 200f;
        //float bomp = 1f - a * Mathf.Abs(Mathf.Sin(5 * Time.time));
        //transform.localScale = new Vector3(bomp, bomp, 1f);
    //}

    public void Set(Score score) {
        var length = score.Difficulty / (3f + score.Difficulty);
        fill.localScale = new Vector3(length, 1f, 1f);
        fillImage.color = ColorFrom(length);
        percentage.text = string.Format("x{0}", combo = score.Combo);
    }

    Color ColorFrom(float length) {
        length = Mathf.Clamp01(length);
        if (length <= 0.2f) { return new Color(1f, length * 5f, 0f); }
        if (length <= 0.4f) { return new Color(1f - (length - 0.2f) * 5f, 1f, 0f); }
        if (length <= 0.6f) { return new Color(0f, 1f, (length - 0.4f) * 5f); }
        if (length <= 0.8f) { return new Color(0f, 1f - (length - 0.6f) * 5f, 1f); }
        return new Color((length - 0.8f) * 5f, 0f, 1f);
    }
}
