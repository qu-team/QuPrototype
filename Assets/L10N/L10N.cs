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
        protected const string EGRAVE = "\x00e8",
                               EACUTE = "\x00e9",
                               AGRAVE = "\x00e0",
                               IGRAVE = "\x00ec",
                               OGRAVE = "\x00f2",
                               UGRAVE = "\x00f9";

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
        LEVEL_LOCKED,
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
        CARD_3_TITLE,
        CARD_3_DESCRIPTION,
        CARD_3_TASK,
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
            { Label.LEVEL_LOCKED, "This level is locked!" },
            { Label.SAVED_QUS, "Saved qUs" },
            { Label.TO_NEXT_LEVEL, "To next level" },
            { Label.FB_NEW_HIGH_SCORE, "New high score on qU!" },
            { Label.FB_SCORED, "I scored {0} points!" },
            { Label.TWIT, "I scored {0} points on qU!" },
            { Label.SELECT_LANGUAGE, "SELECT LANGUAGE" },
            { Label.CARD_1_TITLE, "Colors" },
            { Label.CARD_1_DESCRIPTION, @"Colors are caused by the light reflecting or being absorbed on a surface. 100% absorption creates a black color, while 0% creates a white color."},
            { Label.CARD_1_TASK, "Complete Level 1" },
            { Label.CARD_2_TITLE, "Blue sky and Eyes" },
            { Label.CARD_2_DESCRIPTION, "Blue eyes and sky contain no blue pigment; their color is caused by a phenomenon called Rayleigh scattering" },
            { Label.CARD_2_TASK, "Complete 5 levels" },
            { Label.CARD_3_TITLE, "Black and White" },
            { Label.CARD_3_DESCRIPTION, "Black and white are not ‘conventional’ colors : white is the sum of all colors and black is the lack of them, but only in additive color mixing." },
            { Label.CARD_3_TASK, "Score at least 300 points in 5 levels" },
            { Label.CARD_4_TITLE, "Screens" },
            { Label.CARD_4_DESCRIPTION, "In LED screens, every unit (traditionally called ‘pixel’) is actually made of 3 different leds corresponding to RGB. When we see a 'white pixel', what we actually see are all the three LED active togheter." },
            { Label.CARD_4_TASK, "Achieve at least 2 stars in every level" },
			{ Label.CARD_5_TITLE, "Blacker than Black" },
            { Label.CARD_5_DESCRIPTION, @"There is a material, invented in 2014, that reflects only 0.035% of the visible spectrum, making it almost pure black. It’s called Vantablack and it relies on nanotechnology." },
            { Label.CARD_5_TASK, "Save 80 qU in the same level" },
			{ Label.CARD_6_TITLE, "Wavelength trap" },
            { Label.CARD_6_DESCRIPTION, @"There are some butterfly, belonging to the Morpho genus, that have no pigmentation. They rely on the nanostructure of their wings to have their typical iridescent blue color." },
            { Label.CARD_6_TASK, "Achieve 3 stars in every level" },
			{ Label.CARD_7_TITLE, "Colors and Cultures" },
            { Label.CARD_7_DESCRIPTION, @"In many cultures, white symbolized purity and holyness. In Asian cultures, it symbolizes death and mourn." },
            { Label.CARD_7_TASK, "Complete level 9" },
			{ Label.CARD_8_TITLE, "Colors of Humans" },
            { Label.CARD_8_DESCRIPTION, @"The most important pigment that determines the color of human skin is called melanine. There are some humans with little or no melanine, known as Albinos. " },
            { Label.CARD_8_TASK, "Get the 10th Star" },
			{ Label.CARD_9_TITLE, "Color vision" },
            { Label.CARD_9_DESCRIPTION, @"Human eyes possess two types of light perception cells: Cones and Rods.Cones are responsible for color perception and work better with intense light." },
            { Label.CARD_9_TASK, "Strike a 15x combo" },
			{ Label.CARD_10_TITLE, "Black cats" },
            { Label.CARD_10_DESCRIPTION, @"In some cultures, like UK, Japanese, and Celtic, a black cat symbolized good luck. In other western cultures, black cats are considered a bad omen." },
            { Label.CARD_10_TASK, "Accumulate 2000 points in total" },
            { Label.ANIMATION_1_TEXT, "We need help" },
            { Label.ANIMATION_2_TEXT, "We're lost" },
            { Label.ANIMATION_3_TEXT, "We came here" },
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
            { Label.LEVEL_LOCKED, "Questo livello " + EGRAVE + " bloccato!" },
            { Label.SAVED_QUS, "qU salvati" },
            { Label.TO_NEXT_LEVEL, "Al prossimo livello" },
            { Label.FB_NEW_HIGH_SCORE, "Nuovo punteggio massimo su qU!" },
            { Label.FB_SCORED, "Ho totalizzato {0} punti!" },
            { Label.TWIT, "Ho totalizzato {0} punti su qU!" },
            { Label.SELECT_LANGUAGE, "SELEZIONA LINGUA" },
			{ Label.CARD_1_TITLE, "Colori" },
            { Label.CARD_1_DESCRIPTION, @"I colori vengono generati dalla luce che si riflette o viene assorbita da una superficie. Se viene completamente assorbita la superficie è nera, altrimenti è bianca."},
            { Label.CARD_1_TASK, "Completa il livello 1" },
			{ Label.CARD_2_TITLE, "Occhi e Cielo blu" },
            { Label.CARD_2_DESCRIPTION, "Gli occhi azzurri e il cielo non contengono nessun pigmento blu. A dare loro la caratteristica colorazione è un fenomento che si chiama 'Scattering di Rayleigh'." },
            { Label.CARD_2_TASK, "Completa 5 livelli" },
            { Label.CARD_3_TITLE, "Bianco e Nero" },
            { Label.CARD_3_DESCRIPTION, "Il bianco e il nero non sono colori 'normali': il bianco è la somma di tutti i colori mentre il nero è l'assenza di questi, ma solo in mescolanza additiva" },
            { Label.CARD_3_TASK, "Totalizza almeno 300 punti in 5 livelli" },
            { Label.CARD_4_TITLE, "Schermi" },
            { Label.CARD_4_DESCRIPTION, "" },
			{ Label.CARD_4_TASK, "Ottieni almeno 2 stelle in ogni livello"},
			{ Label.CARD_5_TITLE, "Più nero del nero" },
            { Label.CARD_5_DESCRIPTION, "Esiste un materiale, inventato nel 2014, che riflette solo lo 0.035% della luce visibile, rendendolo quasi puramente nero. Si chiama Vantablack e si basa sulle nanotecnologie." },
			{ Label.CARD_5_TASK, "Salva 80 qU nello stesso livello (non nella stessa partita)"},
			{ Label.CARD_6_TITLE, "Trappola di lunghezza d'onda" },
            { Label.CARD_6_DESCRIPTION, @"Esistono delle farfalle, appartenenti al genere Morpho, che non hanno pigmentazione. Il loro caratteristico colore blu dipende dalla nanostruttura delle loro ali" },
            { Label.CARD_6_TASK, "Ottieni 3 stelle in ogni livello" },
			{ Label.CARD_7_TITLE, "Colori e Culture" },
            { Label.CARD_7_DESCRIPTION, @"In alcune culture, il bianco simbolizza purezza e sacralità. Nelle culture asiatiche, invece, può rappresentare morte e sofferenza" },
            { Label.CARD_7_TASK, "Completa il livello 9" },
			{ Label.CARD_8_TITLE, "Colori degli Umani" },
            { Label.CARD_8_DESCRIPTION, @"Il pigmento più importante che determina il colore della pelle umana si chiama melanina. Ci sono alcuni esseri umani che ne possiedono poca o nessuna, conosciuti come Albini" },
            { Label.CARD_8_TASK, "Ottieni la decima stella" },
			{ Label.CARD_9_TITLE, "Visione dei Colori" },
            { Label.CARD_9_DESCRIPTION, @"Gli esseri umani possiedono due diverse cellule per percepire la luce: i coni e i bastoncelli. I coni sono responsabili per la visione dei colori e funzionano meglio se la luce è intensa" },
            { Label.CARD_9_TASK, "Realizza una combo da 15x " },
			{ Label.CARD_10_TITLE, "Gatti neri" },
            { Label.CARD_10_DESCRIPTION, @"In alcune culture, tra cui la cultura Giapponese e quella Celtica, i gatti neri simbolizzano la fortuna. In altre culture occidentali, invece, sono considerati di cattivo auspicio." },
			{ Label.CARD_10_TASK, "Accumula 200 punti in totale" },
            { Label.ANIMATION_1_TEXT, "Auitaci" },
            { Label.ANIMATION_2_TEXT, "Siamo smarriti" },
            { Label.ANIMATION_3_TEXT, "Arrivammo qui" }
        }) { }
    }
}
