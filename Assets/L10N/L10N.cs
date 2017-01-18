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

    /// <summary>
    /// Returns the string associated to the given label string in the current language.
    /// If the string contains format placeholders, extra arguments can be passed to fill them.
    /// </summary>
    public static string Translate(string label, params object[] arguments) {
        var translated = lang.Translate(label);
        if (arguments.Length == 0) { return translated; }
        return string.Format(translated, arguments);
    }

    /// <summary>
    /// Returns the string associated to the given label in the current language.
    /// If the string contains format placeholders, extra arguments can be passed to fill them.
    /// </summary>
    public static string Translate(Label label, params object[] arguments) {
        return Translate(label.ToString(), arguments);
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
        THEATRE,
        SHARE,
        SCORE,
        CONTINUE,
        FB_NEW_HIGH_SCORE,
        FB_SCORED,
        TWIT
    }

    private class English : Language {
        public English() : base(new Dictionary<Label, string>() {
            { Label.PLAY, Label.PLAY.ToString() },
            { Label.SETTINGS, Label.SETTINGS.ToString() },
            { Label.THEATRE, Label.THEATRE.ToString() },
            { Label.SHARE, "SHARE SCORE" },
            { Label.SCORE, Label.THEATRE.ToString() },
            { Label.CONTINUE, Label.THEATRE.ToString() },
            { Label.FB_NEW_HIGH_SCORE, "New high score on qU!" },
            { Label.FB_SCORED, "I scored {0} points!" },
            { Label.TWIT, "I scored {0} points on qU!" }
        }) { }
    }

    private class Italian : Language {
        public Italian() : base(new English(), new Dictionary<Label, string>() {
            { Label.PLAY, "GIOCA" },
            { Label.SETTINGS, "IMPOSTAZIONI" },
            { Label.THEATRE, "RICORDI" },
            { Label.SHARE, "CONDIVIDI PUNTEGGIO" },
            { Label.SCORE, "PUNTEGGIO" },
            { Label.CONTINUE, "CONTINUA" },
            { Label.FB_NEW_HIGH_SCORE, "Nuovo punteggio massimo su qU!" },
            { Label.FB_SCORED, "Ho totalizzato {0} punti!" },
            { Label.TWIT, "Ho totalizzato {0} punti su qU!" }
        }) { }
    }
}
