using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Add to the root of a UI hierarchy to translate all the Text child components.</summary>
public class L10N : MonoBehaviour {

    static Language lang;

    void Awake() {
        if (lang == null) { lang = LanguageFor(CurrentLanguage); }
    }

    static Language LanguageFor(SystemLanguage language) {
        switch (language) {
            case SystemLanguage.Italian: return new Italian();
            default: return new English();
        }
    }

    public static SystemLanguage CurrentLanguage {
        get {
            if (!PlayerPrefs.HasKey("Language")) { return Application.systemLanguage; }
            switch(PlayerPrefs.GetString("Language")) {
                case "Italian": return SystemLanguage.Italian;
                default: return SystemLanguage.English;
            }
        }

        set {
            lang = LanguageFor(value);
            PlayerPrefs.SetString("Language", value.ToString());
            PlayerPrefs.Save();
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
        MEMORIES,
        COLLECTION,
        BACK,
        LOADING,
        SHARE,
        SCORE,
        CONTINUE,
        RESUME,
        PREV,
        NEXT,
        BEST_SCORE,
        SAVED_QUS,
        TO_NEXT_LEVEL,
        FB_NEW_HIGH_SCORE,
        FB_SCORED,
        TWIT,
        SELECT_LANGUAGE,
        CARD_1_TITLE,
        CARD_1_DESCRIPTION,
        CARD_1_TASK,
        CARD_2_TITLE,
        CARD_2_DESCRIPTION,
        CARD_2_TASK,
        ANIMATION_1_TEXT,
        ANIMATION_2_TEXT,
        ANIMATION_3_TEXT
    }

    private class English : Language {
        public English() : base(new Dictionary<Label, string>() {
            { Label.PLAY, Label.PLAY.ToString() },
            { Label.SETTINGS, Label.SETTINGS.ToString() },
            { Label.MEMORIES, Label.MEMORIES.ToString() },
            { Label.COLLECTION, Label.COLLECTION.ToString() },
            { Label.BACK, Label.BACK.ToString() },
            { Label.LOADING, "Loading..." },
            { Label.SHARE, "SHARE SCORE" },
            { Label.SCORE, Label.MEMORIES.ToString() },
            { Label.CONTINUE, Label.CONTINUE.ToString() },
            { Label.RESUME, Label.RESUME.ToString() },
            { Label.PREV, "⇦" },
            { Label.NEXT, "⇨" },
            { Label.BEST_SCORE, "Best score" },
            { Label.SAVED_QUS, "Saved qUs" },
            { Label.TO_NEXT_LEVEL, "To next level" },
            { Label.FB_NEW_HIGH_SCORE, "New high score on qU!" },
            { Label.FB_SCORED, "I scored {0} points!" },
            { Label.TWIT, "I scored {0} points on qU!" },
            { Label.SELECT_LANGUAGE, "SELECT LANGUAGE" },
            { Label.CARD_1_TITLE, "Light" },
            { Label.CARD_1_DESCRIPTION, "Colors are a result of blabla light" },
            { Label.CARD_1_TASK, "Complete the campaign" },
            { Label.CARD_2_TITLE, "Animals" },
            { Label.CARD_2_DESCRIPTION, "Dogs see everything grey, ugly life" },
            { Label.CARD_2_TASK, "Get three stars in level one" },
            { Label.ANIMATION_1_TEXT, "We need help" },
            { Label.ANIMATION_2_TEXT, "We're lost" },
            { Label.ANIMATION_3_TEXT, "We came here" }
        }) { }
    }

    private class Italian : Language {
        public Italian() : base(new English(), new Dictionary<Label, string>() {
            { Label.PLAY, "GIOCA" },
            { Label.SETTINGS, "IMPOSTAZIONI" },
            { Label.MEMORIES, "RICORDI" },
            { Label.COLLECTION, "COLLEZIONE" },
            { Label.BACK, "INDIETRO" },
            { Label.LOADING, "Caricamento..." },
            { Label.SHARE, "CONDIVIDI PUNTEGGIO" },
            { Label.SCORE, "PUNTEGGIO" },
            { Label.CONTINUE, "CONTINUA" },
            { Label.RESUME, "RIPRENDI" },
            { Label.BEST_SCORE, "Miglior punteggio" },
            { Label.SAVED_QUS, "qU salvati" },
            { Label.TO_NEXT_LEVEL, "Al prossimo livello" },
            { Label.FB_NEW_HIGH_SCORE, "Nuovo punteggio massimo su qU!" },
            { Label.FB_SCORED, "Ho totalizzato {0} punti!" },
            { Label.TWIT, "Ho totalizzato {0} punti su qU!" },
            { Label.SELECT_LANGUAGE, "SELEZIONA LINGUA" },
            { Label.CARD_1_TITLE, "Luce" },
            { Label.CARD_1_DESCRIPTION, "I colori sono il risultato di blabla luce" },
            { Label.CARD_1_TASK, "Completa la campagna" },
            { Label.CARD_2_TITLE, "Animali" },
            { Label.CARD_2_DESCRIPTION, "I cani vedono tutto grigio, brutta vita" },
            { Label.CARD_2_TASK, "Ottieni tre stelle nel primo livello" },
            { Label.ANIMATION_1_TEXT, "Auitaci" },
            { Label.ANIMATION_2_TEXT, "Siamo smarriti" },
            { Label.ANIMATION_3_TEXT, "Arrivammo qui" }
        }) { }
    }
}
