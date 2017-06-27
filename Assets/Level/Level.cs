using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    internal event System.Action OnStart;
    internal event System.Action OnReinitialize;
    internal event System.Action OnOutOfTime;
    // Param: qu's resistance
    internal event System.Action<float> OnFinalClosing;

    const float SIZE = 6f;

    public Shutter shutter;
    public Qu qu;
    public float closingSpeed;
    public Text scoreboard;
    public Feedback feedback;
    public Battery battery;
    public GameObject stars;
    [Tooltip("Color radius decreases like 1/n^difficultyExponent")]
    public float difficultyExponent = 1f;
    public float resistance;
    public uint duration;
    public AudioClip rightAnswerSound;
    public AudioClip wrongAnswerSound;
    // Time when level was reinitialized last
    public float PartialStartTime { get { return partialStartTime; } }
    public int QuSavedThisRun { get { return nQuSavedThisRun; } }

    public bool IsTutorial {
        get;
        internal set;
    }

    internal Timer timer;

    Score scoreAdder = new Score { basePoints = 10, difficultyMultiplier = 4f };
    IColorGenerator colors;
    int score = 0;
    bool finalClosing = false;
    bool playing;
    bool maxScoreReached = false;
    int nQuSavedThisRun = 0;
    // Time when level was reinitialized last
    float partialStartTime;
    Harvester harvester;
    // Keeps track of the number of saved qu, max score, etc (for this session)
    LevelSaveData levelData;

    void Awake() {
        shutter.relativeSize = SIZE;
        shutter.OnColorSelected = MatchQuColor;
        timer = GetComponent<Timer>();
        harvester = Harvester.Instance;
        levelData = new LevelSaveData();
        GameManager.Instance.LevelLoaded();
    }

    void LoadLevelPrefs() {
        var gm = GameManager.Instance;
        var level = gm.Levels[gm.CurrentLevel];
        closingSpeed = closingSpeed * level.bladesSpeed;
        shutter.bladesNumber = level.blades;
        shutter.BackgroundColor = level.bgColor;
        shutter.internalCircleRadius = level.innerRadius;
        resistance = level.quResistance;
        duration = level.duration;
        difficultyExponent = level.difficultyExp;
        CreateColorGenerator();
    }

    void CreateColorGenerator() {
        var gm = GameManager.Instance;
        var level = gm.Levels[gm.CurrentLevel];
        // Create the ColorGenerator
        if (level.number == gm.Levels.Count) {
            colors = new HSLColorGenerator();
            if (level.saturation > 0)
                (colors as HSLColorGenerator).saturation = level.saturation;
            if (level.brightness > 0)
                (colors as HSLColorGenerator).lightness = level.brightness;
        } else {
            colors = new PremadeColorGenerator();
            (colors as PremadeColorGenerator).Initialize();
        }
    }

    public void ShowedWarningRing() {
        Resume();
    }

    void ShowInnerCircle() {
        if(GameManager.Instance.CurrentLevel == 8){
            GameObject.Find("WarningRing").transform.localScale = Vector3.one * 0.76f;
            return;
        }    
        GameObject.Find("WarningRing").transform.localScale = Vector3.one*3f*shutter.internalCircleRadius/(shutter.MaxOpening*1.3f);
    }

    void Start() {
        if (IsTutorial) {
            CreateColorGenerator();
            SetTutorialSettings();
        } else {
            LoadLevelPrefs();
            DisableTutorialGraphics();
        }
        SetupQuAndBladesColors();
        if(shutter.internalCircleRadius > 0.05f){
            Pause();
            ShowInnerCircle();
        }else{
            var go = GameObject.Find("WarningRing");
            go.SetActive(false);
            LogHelper.Debug("Ring", "Ring is "+go+" and is being deactivated");
        }
        timer.Set(duration);
        playing = true;
        partialStartTime = Time.time;
        if (OnStart != null)
            OnStart();
    }

    void SetTutorialSettings() {
        shutter.bladesNumber = 3;
        shutter.BackgroundColor = new Color(45 / 255f, 45 / 255f, 45 / 255f);
        duration = 30;
        resistance = 3;
    }

    void DisableTutorialGraphics() {
        GameObject.Find("Hand").SetActive(false);
        GameObject.Find("Arrow").SetActive(false);
    }

    public Color CurrentQuColor(){
        return qu.Color;
    }

    internal void MatchQuColor(Color color) {
        if (qu.Dead) { return; }
        if (!playing) { return; }
        if (color == qu.Color) { Succeeded(); } else { Failed(); }
        if (!IsTutorial)
            harvester.SaveSingleSessionData(this, color);
        finalClosing = false;
        Reinitialize();
    }

    void Update() {
        if (finalClosing) { return; }
        if (!playing) { return; }
        if (timer.OutOfTime) {
            playing = false;
            StartCoroutine(OutOfTimeAnimation());
        } else if (shutter.opening <= 0f) {
            shutter.opening = 0f;
            finalClosing = true;
            if (OnFinalClosing != null)
                OnFinalClosing(resistance);
            StartCoroutine(FinalClosingAnimation());
        } else if (timer.enabled) {
            shutter.opening -= closingSpeed * Time.deltaTime;
        }
    }

    void Reinitialize() {
        shutter.ResetOpening();
        qu.Restore();
        SetDifficulty();
        SetupQuAndBladesColors();
        partialStartTime = Time.time;
        if (OnReinitialize != null)
            OnReinitialize();
    }

    void Succeeded() {
        ++nQuSavedThisRun;
        feedback.Ok();
        qu.BeHappy();
        GetComponent<AudioSource>().PlayOneShot(rightAnswerSound);
        scoreAdder.Succeeded();
        if (!IsTutorial) {
            score += scoreAdder.Value;
            battery.Set(scoreAdder);
            scoreboard.text = score.ToString();
            levelData.quSaved++;
            levelData.maxCombo = (uint)Mathf.Max(levelData.maxCombo, scoreAdder.Combo);
            int lv = GameManager.Instance.CurrentLevel;
            // Check if we reached the max score
            if (score > levelData.maxScore) {
                levelData.maxScore = score;
                Debug.Assert(GameData.data.levels != null 
                        && GameData.data.levels.Count > GameManager.Instance.CurrentLevel);
                if (score > GameData.data.levels[GameManager.Instance.CurrentLevel].maxScore)
                    maxScoreReached = true;
            }
        }
    }

    void Failed() {
        scoreAdder.Failed();
        battery.Set(scoreAdder);
        feedback.Wrong();
        qu.BeScared();
        timer.TimePenality(SecondsOfPenality);
        GetComponent<AudioSource>().PlayOneShot(wrongAnswerSound);
    }

    float SecondsOfPenality { get { return 5f - (Time.time - partialStartTime); } }

    void SetupQuAndBladesColors() {
        var rcolors = RandomColors();
        var index = Random.Range(0, (int)shutter.bladesNumber);
        shutter.SetBladeColors(rcolors);
        qu.Color = rcolors[index];
    }

    Color[] RandomColors() {
        Color[] randomColors;
        do {
            randomColors = colors.Generate((int)shutter.bladesNumber);
            Debug.Assert(randomColors.Length == shutter.bladesNumber);
        } while (AnyColorIsSimilarToBackground(randomColors));
        return randomColors;
    }

    bool AnyColorIsSimilarToBackground(Color[] cols) {
        const float threshold = 5f / 256f;
        var background = shutter.BackgroundColor;
        foreach (var color in cols) {
            if (Mathf.Abs(background.r - color.r) >= threshold) { continue; }
            if (Mathf.Abs(background.g - color.g) >= threshold) { continue; }
            if (Mathf.Abs(background.b - color.b) >= threshold) { continue; }
            return true;
        }
        return false;
    }

    void SetDifficulty() {
        //colors.Difficulty = Mathf.Pow(scoreAdder.Difficulty, difficultyExponent);
        colors.Difficulty = scoreAdder.Difficulty * difficultyExponent;
    }

    IEnumerator FinalClosingAnimation() {
        float finalClosingTime=0;
        if (finalClosing) {
            qu.StretchEyes();
            yield return null;
        }
        while(finalClosingTime<resistance){
            if(!Paused){
                finalClosingTime+= Time.deltaTime;
            }
            yield return null;
        }
        if (finalClosing) {
            qu.Die();
            Failed();
        }
        if (finalClosing) {
            yield return new WaitForSeconds(0.5f);
            Reinitialize();
            qu.StopDying();
            qu.OpenEyes();
            finalClosing = false;
        }
    }

    public void Pause() {
        timer.enabled = false;
    }

    public void Resume() {
        timer.enabled = true;
    }

    public bool Paused { get { return !timer.enabled; } }

    public void Quit() {
        GameManager.Instance.Back();
    }

    IEnumerator OutOfTimeAnimation() {
        var time = 2f;
        if (OnOutOfTime != null)
            OnOutOfTime();
        EnableStars();
        while (time > 0) {
            Camera.main.orthographicSize *= 1.1f;
            SetStarsAlpha(Mathf.Min(time, 2f - time));
            time -= Time.deltaTime;
            yield return null;
        }
        SetStarsAlpha(0f);
        if (!IsTutorial) {
            LogHelper.Info(this, "Saving data");
            SaveData();
        } else {
            LogHelper.Info(this, "Level is tutorial: not saving data");
            PlayerPrefs.SetInt(Preferences.PLAYED_TUTORIAL, 1);
        }
        LoadNextScene();
    }

    void EnableStars() {
        stars.SetActive(true);
        var starScores = GameManager.Instance.Levels[GameManager.Instance.CurrentLevel].stars;
        if (starScores.first > score) { stars.transform.Find("Star1").gameObject.SetActive(false); }
        if (starScores.second > score) { stars.transform.Find("Star2").gameObject.SetActive(false); }
        if (starScores.third > score) { stars.transform.Find("Star3").gameObject.SetActive(false); }
    }

    void SetStarsAlpha(float alpha) {
        foreach (Transform star in stars.transform) {
            var image = star.gameObject.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }

    void SaveData() {
        harvester.SendStoredData(this);
        var game = GameManager.Instance;
        var lv = game.CurrentLevel;
        GameData.data.levels[lv] = GameData.data.levels[lv].Overwrite(levelData);
        // Check if we unlocked next level
        if (lv == GameData.data.curLevelUnlocked && GameData.data.levels[lv].quSaved >= game.Levels[lv].quToNextLevel) {
            ++GameData.data.curLevelUnlocked;
        }
        GameData.Save();
    }

    void LoadNextScene() {
        GameManager.Instance.goToShare = maxScoreReached;    
        var nextScene = (!IsTutorial) ? QuScene.SCORE : QuScene.MAP;
        GameManager.Instance.ShowUnlockedCardsThenGoTo(nextScene);
    }
}
