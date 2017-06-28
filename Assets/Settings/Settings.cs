using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    public AudioClip changeLanguageSound;
    public GameObject erasePanel;
    public Text eraseYes;

    Qu qu;
    SpriteRenderer halo;
    Text selectLanguageLabel;
    SystemLanguage selectedLanguage;
    SystemLanguage prevLanguage;
    Dictionary<SystemLanguage, Sprite> flags;

    void Awake() {
        qu = FindObjectOfType<Qu>();
        halo = GameObject.Find("FlagHalo").GetComponent<SpriteRenderer>();
        halo.gameObject.SetActive(false);
        selectLanguageLabel = GameObject.Find("SelectLanguage").GetComponent<Text>();
        flags = new Dictionary<SystemLanguage, Sprite>();
        foreach (var language in FLAGS.Keys) {
            flags[language] = Resources.Load<Sprite>("Flags/" + FLAGS[language]);
        }
    }

    void Start() {
        prevLanguage = selectedLanguage = L10N.CurrentLanguage;
        SetQuFlag();
        Gestures.GestureSystem.dispatcher.OnSwipeStart += (Gestures.Swipe _) => {
            prevLanguage = selectedLanguage;
            ChangeLanguage();
        };
    }

    void SetQuFlag() {
        qu.GetComponent<SpriteRenderer>().sprite = flags[selectedLanguage];
    }

    public void ChangeLanguage() {
        for (int i = 0; i < LANGUAGES.Count; ++i) {
            if (LANGUAGES[i] == selectedLanguage) {
                selectedLanguage = LANGUAGES[(i + 1) % LANGUAGES.Count];
                break;
            }
        }
        L10N.CurrentLanguage = selectedLanguage;
        selectLanguageLabel.text = L10N.Translate(L10N.Label.SELECT_LANGUAGE);
        GameObject.Find("BackButton").GetComponentInChildren<Text>().text = L10N.Translate(L10N.Label.BACK);
        GameObject.Find("SaveButton").GetComponentInChildren<Text>().text = L10N.Translate(L10N.Label.SAVE);
        GameObject.Find("EraseButton").GetComponentInChildren<Text>().text = L10N.Translate(L10N.Label.ERASE_DATA);
        erasePanel.transform.Find("EraseConfirmText").GetComponent<Text>().text = L10N.Translate(L10N.Label.ERASE_DATA_CONFIRM);
        eraseYes.text = L10N.Translate(L10N.Label.YES);
        SetQuFlag();
        qu.BeHappy();
        StartCoroutine(FireHalo());
        AudioSource.PlayClipAtPoint(changeLanguageSound, Camera.main.transform.position);
    }

    IEnumerator FireHalo() {
        var language = selectedLanguage;
        halo.sprite = flags[language];
        var scale = 0.1f;
        halo.transform.localScale = new Vector3(scale, scale);
        halo.color = Color.white;
        var alpha = 1f;
        halo.gameObject.SetActive(true);
        yield return null;
        while (alpha >= 0.01f && language == selectedLanguage) {
            scale += 2f * Time.deltaTime;
            alpha -= 2f * Time.deltaTime;
            halo.transform.localScale = new Vector3(scale, scale);
            halo.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        if (language == selectedLanguage) { halo.gameObject.SetActive(false); }
    }

    public void Back() {
        L10N.CurrentLanguage = prevLanguage;
        GameManager.Instance.Back();
    }

    public void Save() {
        GameManager.Instance.Back();
    }

    static readonly Dictionary<SystemLanguage, string> FLAGS = new Dictionary<SystemLanguage, string>() {
        {SystemLanguage.English, "english" },
        {SystemLanguage.Italian, "italian" }
    };

    static readonly List<SystemLanguage> LANGUAGES = new List<SystemLanguage>(FLAGS.Keys);

    public void EraseOpenDialog() {
        erasePanel.SetActive(true);
    }

    public void EraseYes() {
        GameData.Erase();
        GameData.Load();
        var uuid = PlayerPrefs.GetString(Protocol.UUID_KEY, "");
        PlayerPrefs.DeleteAll();
	GameManager.Instance.TotalTimePlayed = 0;
#if !DEBUG
        PlayerPrefs.SetString(Protocol.UUID_KEY, uuid);
#endif
        var eraseBtn = GameObject.Find("EraseButton");
        eraseBtn.GetComponentInChildren<Text>().text = L10N.Translate(L10N.Label.DATA_ERASED);
        eraseBtn.GetComponent<Button>().interactable = false;
        erasePanel.SetActive(false);
    }

    public void EraseNo() {
        erasePanel.SetActive(false);
    }
}
