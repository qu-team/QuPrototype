using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    Qu qu;
    SpriteRenderer halo;
    Button back;
    Text selectLanguageLabel;
    Text dataCollectionLabel;
    Text dataQuestionLabel;
    SystemLanguage selectedLanguage;
    Dictionary<SystemLanguage, Sprite> flags;

    void Awake() {
        qu = FindObjectOfType<Qu>();
        halo = GameObject.Find("FlagHalo").GetComponent<SpriteRenderer>();
        halo.gameObject.SetActive(false);
        back = GameObject.Find("BackButton").GetComponent<Button>();
        selectLanguageLabel = GameObject.Find("SelectLanguage").GetComponent<Text>();
        dataCollectionLabel = GameObject.Find("DataCollection").GetComponent<Text>();
        dataQuestionLabel = GameObject.Find("DataQuestion").GetComponent<Text>();
        flags = new Dictionary<SystemLanguage, Sprite>();
        foreach (var language in FLAGS.Keys) {
            flags[language] = Resources.Load<Sprite>("Flags" + System.IO.Path.DirectorySeparatorChar + FLAGS[language]);
        }
    }

    void Start() {
        selectedLanguage = L10N.CurrentLanguage;
        SetQuFlag();
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
        dataCollectionLabel.text = L10N.Translate(L10N.Label.DATA_COLLECTION);
        dataQuestionLabel.text = L10N.Translate(L10N.Label.DATA_QUESTION);
        back.GetComponentInChildren<Text>().text = L10N.Translate(L10N.Label.BACK);
        SetQuFlag();
        qu.BeHappy();
        StartCoroutine(FireHalo());
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
        GameManager.Instance.Back();
    }

    public void DataCollectionUserAgreement(bool userAgrees) {
        print(userAgrees);
        // TODO: set user preference about data collection
    }

    static readonly Dictionary<SystemLanguage, string> FLAGS = new Dictionary<SystemLanguage, string>() {
        {SystemLanguage.English, "english" },
        {SystemLanguage.Italian, "italian" }
    };

    static readonly List<SystemLanguage> LANGUAGES = new List<SystemLanguage>(FLAGS.Keys);
}
