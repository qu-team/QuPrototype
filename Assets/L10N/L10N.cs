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

    public void ApplyTranslationTo(GameObject obj) {
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
                               IGRAVE_CAPITAL = "\x00cc",
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
        CARD_UNLOCKED,
        CARD_1_TITLE,
        CARD_1_DESCRIPTION,
        CARD_1_TASK,
        CARD_2_TITLE,
        CARD_2_DESCRIPTION,
        CARD_2_TASK,
        CARD_3_TITLE,
        CARD_3_DESCRIPTION,
        CARD_3_TASK,
        CARD_4_TITLE,
        CARD_4_DESCRIPTION,
        CARD_4_TASK,
        CARD_5_TITLE,
        CARD_5_DESCRIPTION,
        CARD_5_TASK,
        CARD_6_TITLE,
        CARD_6_DESCRIPTION,
        CARD_6_TASK,
        CARD_7_TITLE,
        CARD_7_DESCRIPTION,
        CARD_7_TASK,
        CARD_8_TITLE,
        CARD_8_DESCRIPTION,
        CARD_8_TASK,
        CARD_9_TITLE,
        CARD_9_DESCRIPTION,
        CARD_9_TASK,
        CARD_10_TITLE,
        CARD_10_DESCRIPTION,
        CARD_10_TASK,
        CARD_11_TITLE,
        CARD_11_DESCRIPTION,
        CARD_11_TASK,
        CARD_12_TITLE,
        CARD_12_DESCRIPTION,
        CARD_12_TASK,
        CARD_13_TITLE,
        CARD_13_DESCRIPTION,
        CARD_13_TASK,
        CARD_14_TITLE,
        CARD_14_DESCRIPTION,
        CARD_14_TASK,
        CARD_15_TITLE,
        CARD_15_DESCRIPTION,
        CARD_15_TASK,
        CARD_16_TITLE,
        CARD_16_DESCRIPTION,
        CARD_16_TASK,
        CARD_17_TITLE,
        CARD_17_DESCRIPTION,
        CARD_17_TASK,
        CARD_18_TITLE,
        CARD_18_DESCRIPTION,
        CARD_18_TASK,
        ANIMATION_1_TEXT,
        ANIMATION_2_TEXT,
        ANIMATION_3_TEXT,
        ANIMATION_4_TEXT,
        ANIMATION_5_TEXT,
        ANIMATION_6_TEXT,
        ANIMATION_7_TEXT,
        ANIMATION_8_TEXT,
        ANIMATION_9_TEXT,
        ANIMATION_10_TEXT,
        INTRO1,
        INTRO2,
        INTRO3,
        SAVE,
        ERASE_DATA,
        ERASE_DATA_CONFIRM,
        NO,
        YES,
        DATA_ERASED
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
            { Label.LEVEL_LOCKED, "Save $$ more qU in the previous level to unlock!" },
            { Label.SAVED_QUS, "Saved qUs" },
            { Label.TO_NEXT_LEVEL, "To next level" },
            { Label.FB_NEW_HIGH_SCORE, "New high score on qU!" },
            { Label.FB_SCORED, "I scored {0} points!" },
            { Label.TWIT, "I scored {0} points on qU!" },
            { Label.SELECT_LANGUAGE, "SELECT LANGUAGE" },
            { Label.CARD_UNLOCKED, "CARD UNLOCKED!" },
            { Label.CARD_1_TITLE, "Colors" },
            { Label.CARD_1_DESCRIPTION, @"Colors are caused by the light reflecting or being absorbed on a surface. 100% absorption creates a black color, while 0% creates a white color."},
            { Label.CARD_1_TASK, "Complete Level 1" },
            { Label.CARD_2_TITLE, "Blue sky and Eyes" },
            { Label.CARD_2_DESCRIPTION, "Blue eyes and sky contain no blue pigment; their color is caused by a phenomenon called Rayleigh scattering." },
            { Label.CARD_2_TASK, "Complete 5 levels" },
            { Label.CARD_3_TITLE, "Black and White" },
            { Label.CARD_3_DESCRIPTION, "Black and white are not \"conventional\" colors: white is the sum of all colors and black is the lack of them, but only in additive color mixing." },
            { Label.CARD_3_TASK, "Score at least 300 points in 5 levels" },
            { Label.CARD_4_TITLE, "Screens" },
            { Label.CARD_4_DESCRIPTION, "LED screen use the addition of colors and their structure in order to create different colors" },
            { Label.CARD_4_TASK, "Achieve at least 2 stars in every level" },
            { Label.CARD_5_TITLE, "Blacker than Black" },
            { Label.CARD_5_DESCRIPTION, @"There is a material, invented in 2014, that reflects only 0.035% of the visible spectrum, making it almost pure black. It’s called Vantablack and it relies on nanotechnology." },
            { Label.CARD_5_TASK, "Save 80 qU in the same level" },
            { Label.CARD_6_TITLE, "Wavelength trap" },
            { Label.CARD_6_DESCRIPTION, @"There are some butterfly, belonging to the Morpho genus, that have no pigmentation. They rely on the nanostructure of their wings to have their typical iridescent blue color." },
            { Label.CARD_6_TASK, "Achieve 3 stars in every level" },
            { Label.CARD_7_TITLE, "Colors and Cultures" },
            { Label.CARD_7_DESCRIPTION, @"In many cultures, white symbolized purity and holyness. In Asian cultures, it symbolizes death and mourn." },
            { Label.CARD_7_TASK, "Complete the bleached level" },
            { Label.CARD_8_TITLE, "Colors of Humans" },
            { Label.CARD_8_DESCRIPTION, @"The most important pigment that determines the color of human skin is called melanine. There are some humans with little or no melanine, known as Albinos. " },
            { Label.CARD_8_TASK, "Get the 10th Star" },
            { Label.CARD_9_TITLE, "Color vision" },
            { Label.CARD_9_DESCRIPTION, @"Human eyes possess two types of light perception cells: Cones and Rods.Cones are responsible for color perception and work better with intense light." },
            { Label.CARD_9_TASK, "Strike a 15x combo" },
            { Label.CARD_10_TITLE, "Black cats" },
            { Label.CARD_10_DESCRIPTION, @"In some cultures, like UK, Japanese, and Celtic, a black cat symbolized good luck. In other western cultures, black cats are considered a bad omen." },
            { Label.CARD_10_TASK, "Accumulate 2000 points in total" },
            { Label.CARD_11_TITLE, "Colorblindness" },
            { Label.CARD_11_DESCRIPTION, @"Some people have trouble distinguishing some colors, in particular red and green." },
            { Label.CARD_11_TASK, "Complete the desaturated level" },
            { Label.CARD_12_TITLE, "Rainbow animals" },
            { Label.CARD_12_DESCRIPTION, @"There is a specie of shrimp, known as mantis shrimp, that possesses 16 color receptors. Humans only have three." },
            { Label.CARD_12_TASK, "Achieve 3 stars in 5 levels" },
            { Label.CARD_13_TITLE, "Prism" },
            { Label.CARD_13_DESCRIPTION, @"White light can be split in its colorful components using a prism: an object with particular angles that is able to change the direction of light based on its wavelength." },
            { Label.CARD_13_TASK, "Complete the last level" },
            { Label.CARD_14_TITLE, "Screen and paper" },
            { Label.CARD_14_DESCRIPTION, @"Digital artists always need to check how their work looks in CYMK (Cyan-Yellow-Magenta-blacK) color space, since screen colors are in RGB (Red-Green-Blue) space but printers work in CYMK." },
            { Label.CARD_14_TASK, "Watch all the cutscenes in a row in the \"Memories\" section" },
            { Label.CARD_15_TITLE, "Communication" },
            { Label.CARD_15_DESCRIPTION, @"Colors play an incredible role in communication. Many designers know how to color something in order to make it more appealing for a certain slice of the population." },
            { Label.CARD_15_TASK, "Save at least 40 qUs in every level" },
            { Label.CARD_16_TITLE, "Hot 'n Cold" },
            { Label.CARD_16_DESCRIPTION, @"We traditionally associate warmth with long wavelength colors like red, orange and yellow; it is not commonly known that the hotter an object is, the more bluish will be the light emitted." },
            { Label.CARD_16_TASK, "Play for 3 hours(total)" },
            { Label.CARD_17_TITLE, "Fireworks" },
            { Label.CARD_17_DESCRIPTION, @"The color of the firework is given by the chemical composition of the powder it burns: Lithium cloride for magenta, copper salts for blue and greens, and so on." },
            { Label.CARD_17_TASK, "Save 100 qU in total" },
            { Label.CARD_18_TITLE, "qU spins" },
            { Label.CARD_18_DESCRIPTION, @"Can you tell wich qU is upside down?" },
            { Label.CARD_18_TASK, "score more than 500 in a level" },
            { Label.ANIMATION_1_TEXT, "We need help" },
            { Label.ANIMATION_2_TEXT, "We're lost" },
            { Label.ANIMATION_3_TEXT, "We came here" },
            { Label.ANIMATION_4_TEXT, "But we can't stay" },
            { Label.ANIMATION_5_TEXT, "We wanted to help" },
            { Label.ANIMATION_6_TEXT, "We saw your world" },
            { Label.ANIMATION_7_TEXT, "We brought the colors" },
            { Label.ANIMATION_8_TEXT, "It's time to go gome" },
            { Label.ANIMATION_9_TEXT, "Get us home" },
            { Label.ANIMATION_10_TEXT, "Get us home" },
            { Label.INTRO1, "Please" },
            { Label.INTRO2, "Is anyone there?"},
            { Label.INTRO3, "We can't make it alone"},
            { Label.SAVE, Label.SAVE.ToString() },
            { Label.ERASE_DATA, "Erase data" },
            { Label.ERASE_DATA_CONFIRM, "Erase all game data?" },
            { Label.NO, Label.NO.ToString() },
            { Label.YES, Label.YES.ToString() },
            { Label.DATA_ERASED, "Game data erased" },
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
            { Label.LEVEL_LOCKED, "Salva ancora $$ qU nel livello precedente per sbloccare!" },
            { Label.SAVED_QUS, "qU salvati" },
            { Label.TO_NEXT_LEVEL, "Al prossimo livello" },
            { Label.FB_NEW_HIGH_SCORE, "Nuovo punteggio massimo su qU!" },
            { Label.FB_SCORED, "Ho totalizzato {0} punti!" },
            { Label.TWIT, "Ho totalizzato {0} punti su qU!" },
            { Label.SELECT_LANGUAGE, "SELEZIONA LINGUA" },
            { Label.CARD_UNLOCKED, "CARTA SBLOCCATA!" },
            { Label.CARD_1_TITLE, "Colori" },
            { Label.CARD_1_DESCRIPTION, @"I colori vengono generati dalla luce che si riflette o viene assorbita da una superficie. Se viene completamente assorbita la superficie " + EGRAVE + " nera, altrimenti " + EGRAVE + " bianca."},
            { Label.CARD_1_TASK, "Completa il livello 1" },
            { Label.CARD_2_TITLE, "Occhi e Cielo blu" },
            { Label.CARD_2_DESCRIPTION, "Gli occhi azzurri e il cielo non contengono alcun pigmento blu. A dare loro la caratteristica colorazione " + EGRAVE + " un fenomento che si chiama \"Scattering di Rayleigh\"." },
            { Label.CARD_2_TASK, "Completa 5 livelli" },
            { Label.CARD_3_TITLE, "Bianco e Nero" },
            { Label.CARD_3_DESCRIPTION, "Il bianco e il nero non sono colori \"normali\": il bianco " + EGRAVE + " la somma di tutti i colori mentre il nero " + EGRAVE + " l'assenza di questi, ma solo in mescolanza additiva." },
            { Label.CARD_3_TASK, "Totalizza almeno 300 punti in 5 livelli" },
            { Label.CARD_4_TITLE, "Schermi" },
            { Label.CARD_4_DESCRIPTION, "Gli schermi a LED sfruttano la somma dei colori e la loro struttura per creare molti colori dello spettro visibile." },
            { Label.CARD_4_TASK, "Ottieni almeno 2 stelle in ogni livello"},
            { Label.CARD_5_TITLE, "Pi" + UGRAVE + " nero del nero" },
            { Label.CARD_5_DESCRIPTION, "Esiste un materiale, inventato nel 2014, che riflette solo lo 0.035% della luce visibile, risultando quindi quasi puramente nero. Si chiama Vantablack e si basa sulle nanotecnologie." },
            { Label.CARD_5_TASK, "Salva 80 qU nello stesso livello (non nella stessa partita)"},
            { Label.CARD_6_TITLE, "Trappola di lunghezza d'onda" },
            { Label.CARD_6_DESCRIPTION, @"Esistono delle farfalle, appartenenti al genere Morpho, che non hanno pigmentazione. Il loro caratteristico colore blu dipende dalla nanostruttura delle loro ali." },
            { Label.CARD_6_TASK, "Ottieni 3 stelle in ogni livello" },
            { Label.CARD_7_TITLE, "Colori e Culture" },
            { Label.CARD_7_DESCRIPTION, @"In alcune culture, il bianco simbolizza purezza e sacralit" + AGRAVE + ". Nelle culture asiatiche, invece, pu" + OGRAVE + " rappresentare morte e sofferenza." },
            { Label.CARD_7_TASK, "Completa il livello scolorito" },
            { Label.CARD_8_TITLE, "Colori degli Umani" },
            { Label.CARD_8_DESCRIPTION, @"Il pigmento pi" + UGRAVE + " importante che determina il colore della pelle umana si chiama melanina. Ci sono alcuni esseri umani che ne possiedono poca o nessuna, conosciuti come Albini." },
            { Label.CARD_8_TASK, "Ottieni la decima stella" },
            { Label.CARD_9_TITLE, "Visione dei Colori" },
            { Label.CARD_9_DESCRIPTION, @"Gli esseri umani possiedono due diverse cellule per percepire la luce: i coni e i bastoncelli. I coni sono responsabili per la visione dei colori e funzionano meglio se la luce " + EGRAVE + " intensa." },
            { Label.CARD_9_TASK, "Realizza una combo da 15x " },
            { Label.CARD_10_TITLE, "Gatti neri" },
            { Label.CARD_10_DESCRIPTION, @"In alcune culture, tra cui la cultura Giapponese e quella Celtica, i gatti neri simbolizzano la fortuna. In altre culture occidentali, invece, sono considerati di cattivo auspicio." },
            { Label.CARD_10_TASK, "Accumula 200 punti in totale" },
            { Label.CARD_11_TITLE, "Daltonismo" },
            { Label.CARD_11_DESCRIPTION, @"Alcune persone hanno difficolt" + AGRAVE + " a distinguere determinati colori, in particolare rosso e verde." },
            { Label.CARD_11_TASK, "Completa il livello desaturato" },
            { Label.CARD_12_TITLE, "Animali arcobaleno" },
            { Label.CARD_12_DESCRIPTION, "Esiste un crostaceo, comunemente detto \"Canocchia\", che ha 16 recettori per i colori nell'apparato visivo. Gli esseri umani ne hanno solo tre." },
            { Label.CARD_12_TASK, "Ottieni 3 stelle in 5 livelli" },
            { Label.CARD_13_TITLE, "Prismi" },
            { Label.CARD_13_DESCRIPTION, @"La luce bianca pu" + OGRAVE + " essere divisa nelle sue colorate componenti usando un prisma: un oggetto con angoli particolari in grado di cambiare la direzione della luce in base alla lunghezza d'onda." },
            { Label.CARD_13_TASK, "Completa l'ultimo livello" },
            { Label.CARD_14_TITLE, "Schermi e carta " },
            { Label.CARD_14_DESCRIPTION, @"Gli artisti che lavorano in digitale devono sempre controllare il loro lavoro nello spazio di colore CYMK, che " + EGRAVE + " quello usato dalle stampanti. Gli schermi lavorano invece in RGB." },
            { Label.CARD_14_TASK, "Guarda tutti i filmati in una volta sola nella sezione \"Ricordi\"" },
            { Label.CARD_15_TITLE, "Comunicazione" },
            { Label.CARD_15_DESCRIPTION, @"I colori giocano un importantissimo ruolo nella comunicazione. Molti designers sanno come sfruttarli per rendere pi" + UGRAVE + " attraente un prodotto per determinate persone." },
            { Label.CARD_15_TASK, "Salva almeno 40 qU in ogni livello" },
            { Label.CARD_16_TITLE, "Caldo e freddo" },
            { Label.CARD_16_DESCRIPTION, "Generalmente i colori con una lunghezza d'onda maggiore, vengono detti \"caldi\", mentre gli altri vengono definiti \"freddi\". In pochi sanno che pi" + UGRAVE + " un oggetto " + EGRAVE + " caldo, pi" + UGRAVE + " vira verso il blu la luce che emette." },
            { Label.CARD_16_TASK, "Gioca per almeno 3 ore (in totale)" },
            { Label.CARD_17_TITLE, "Fuochi d'artificio" },
            { Label.CARD_17_DESCRIPTION, @"Il colore dei fuochi d'artificio " + EGRAVE + " determinato dalla composizione chimica della polvere che brucia: Cloruro di litio per il magenta, sali di rame per blu e verde, ecc." },
            { Label.CARD_17_TASK, "Salva 100 qU in totale" },
            { Label.CARD_18_TITLE, "Spin dei qU" },
            { Label.CARD_18_DESCRIPTION, @"Sapresti dire quale dei due qU " + EGRAVE + " a testa in gi" + UGRAVE + "?" },
            { Label.CARD_18_TASK, "Ottieni pi" + UGRAVE + " di 500 punti in un livello" },
            { Label.ANIMATION_1_TEXT, "Aiutaci" },
            { Label.ANIMATION_2_TEXT, "Siamo smarriti" },
            { Label.ANIMATION_3_TEXT, "Arrivammo qui" },
            { Label.ANIMATION_4_TEXT, "Ma non possiamo rimanere" },
            { Label.ANIMATION_5_TEXT, "Volevamo aiutare" },
            { Label.ANIMATION_6_TEXT, "Ora dobbiamo tornare a casa" },
            { Label.ANIMATION_7_TEXT, "Abbiamo visto il vostro mondo" },
            { Label.ANIMATION_8_TEXT, "Abbiamo portato i colori" },
            { Label.ANIMATION_9_TEXT, "Aiutaci" },
            { Label.ANIMATION_10_TEXT, "Riportaci a casa" },
            { Label.INTRO1, "Per favore" },
            { Label.INTRO2, "C'"+ EGRAVE+" nessuno?"},
            { Label.INTRO3, "Non ce la possiamo fare\n da soli"},
            { Label.SAVE, "SALVA" },
            { Label.ERASE_DATA, "Cancella dati" },
            { Label.ERASE_DATA_CONFIRM, "Cancellare tutti\ni dati di gioco?" },
            { Label.NO, Label.NO.ToString() },
            { Label.YES, "S" + IGRAVE_CAPITAL },
            { Label.DATA_ERASED, "Dati cancellati" },
        }) { }
    }
}
