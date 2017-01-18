using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Add to the root of a UI hierarchy to translate all the Text child components.</summary>
public class L10N : MonoBehaviour {

    static Language lang;

    void Awake() {
        if (lang == null) { lang = LanguageFor(Application.systemLanguage); }
    }

    static Language LanguageFor(SystemLanguage language) {
        switch (language) {
            case SystemLanguage.Italian: return new Italian();
            default: return new English();
        }
    }

    public static string Translate(string label) {
        return lang.Translate(label);
    }

    void Start() {
        ApplyTranslationTo(gameObject);
    }

    void ApplyTranslationTo(GameObject obj) {
        var text = obj.GetComponent<Text>();
        if (text != null) { ApplyTranslationTo(text); }
        foreach (Transform child in obj.transform) { ApplyTranslationTo(child.gameObject); }
    }

    void ApplyTranslationTo(Text text) {
        text.text = lang.Translate(text.text);
    }

    private abstract class Language {

        readonly IDictionary<string, string> translations;
        readonly Language fallback;

        public Language(Language fallback, IDictionary<Label, string> translations) {
            this.translations = new Dictionary<string, string>();
            foreach (Label text in translations.Keys) { this.translations[text.ToString()] = translations[text]; }
            this.fallback = fallback;
        }

        public Language(IDictionary<Label, string> translations) : this(null, translations) { }

        public string Translate(string label) {
            if (translations.ContainsKey(label)) { return translations[label]; }
            if (fallback != null) { return fallback.Translate(label); }
            return label;
        }
    }

    public enum Label {
        PLAY,
        SETTINGS,
        MEMORIES,
        CARDS
    }

    private class English : Language {
        public English() : base(new Dictionary<Label, string>() {
            { Label.PLAY, Label.PLAY.ToString() },
            { Label.SETTINGS, Label.SETTINGS.ToString() },
            { Label.MEMORIES, Label.MEMORIES.ToString() },
            { Label.CARDS, "CARD COLLECTION" },
        }) { }
    }

    private class Italian : Language {
        public Italian() : base(new English(), new Dictionary<Label, string>() {
            { Label.PLAY, "GIOCA" },
            { Label.SETTINGS, "IMPOSTAZIONI" },
            { Label.MEMORIES, "RICORDI" },
            { Label.CARDS, "COLLEZIONE DI CARTE" },
        }) { }
    }
}
