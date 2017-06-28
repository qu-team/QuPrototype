using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum QuScene {
    MENU,
    GAME,
    CARD_COLLECTION,
    CUT_COLLECTION,
    CUT_GAME,
    MAP,
    SETTINGS,
    SHARE,
    SCORE,
    TUTORIAL,
    UNLOCK
}

public class GameManager : MonoBehaviour {
    public bool goToShare;
    public static GameManager Instance {
        get;
        private set;
    }

    private QuScene currentState;
    private LevelsData levels;
    private Dictionary<QuScene,string> sceneNames = new Dictionary<QuScene,string> {
        {QuScene.MENU, "Menu"},
        {QuScene.CARD_COLLECTION, "CardCollection"},
        {QuScene.CUT_GAME, "Animations"},
        {QuScene.CUT_COLLECTION, "Animations"},
        {QuScene.MAP, "MapScene"},
        {QuScene.GAME, "Level"},
        {QuScene.SETTINGS, "Settings"},
        {QuScene.SHARE, "ShareScore"},
        {QuScene.TUTORIAL, "Level"},
        {QuScene.UNLOCK, "UnlockScene"},
        {QuScene.SCORE, "Score"}
    };

    //Animation screen vars
    public int currAnimation = 0;
    public bool ignoreNextEscape = false;

    private AnimationController currAnimController;
    private int curLevel;

    float totalTimePlayed;
    public float TotalTimePlayed {
        get { return totalTimePlayed + Time.time; }
        set { totalTimePlayed = value; }
    }

    public LevelsData Levels {
        get { return levels; }
        private set { levels = value; }
    }

    public int CurrentLevel {
        get { return curLevel; }
    }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
        currentState = QuScene.MENU;
        levels = new LevelsData("levels.json");
        totalTimePlayed = PlayerPrefs.GetFloat(Preferences.TIME_PLAYED, 0);
    }

    void Start() {
        // Load player stats
        if (!GameData.Load())
            LogHelper.Warn(this, "Game data was not loaded from save file.");
        // Send locally cached data to the server
        Harvester.Instance.SendLocalData(this);
    }

    void Update() {
        if (currentState != QuScene.CARD_COLLECTION && Input.GetKeyUp(KeyCode.Escape)) {
            Back();
        }
    }

    void OnApplicationQuit() {
        // Save total time played
        PlayerPrefs.SetFloat(Preferences.TIME_PLAYED, TotalTimePlayed);
        GameData.Save();
    }

    public void Back() {
        switch (currentState) {
        case QuScene.CARD_COLLECTION:
        case QuScene.CUT_COLLECTION:
        case QuScene.MAP:
        case QuScene.SETTINGS:
            LoadScene(QuScene.MENU);
            break;
        case QuScene.GAME:
        case QuScene.CUT_GAME:
            LoadScene(QuScene.MAP); 
            break;
        case QuScene.MENU:
            Application.Quit();
            break;
        default:
            break;
        }
    }

#region Animations
    public void AnimationFinishedLoading(AnimationController controller) {
        controller.IngameCut = currentState == QuScene.CUT_GAME;
        if (currentState == QuScene.CUT_GAME)
            controller.PlayAnimation(currAnimation);
        else
            controller.PlayAnimation(0);
    }

    public void AnimationFinished(AnimationController currAnimController) {
        switch (currentState) {
        case QuScene.CUT_COLLECTION:
            currAnimController.NextAnimation();
            break;
        default:
            LoadScene(QuScene.GAME);
            break;
        }
    }
#endregion

    public void LoadScene(QuScene scene){
        currentState = scene;
        switch(scene){
            case QuScene.GAME:
            case QuScene.TUTORIAL:
                SceneManager.LoadSceneAsync(sceneNames[scene]);
                break;
            default:
                SceneManager.LoadScene(sceneNames[scene]);
                break;
        }
    }

#region CardCollection

    public void CardCollectionLoaded(CardCollectionManager manager) {
        manager.ShowCardList();
    }

#endregion

#region Map
    bool playingNotTheLastLevel = false;

    public void MapFinishedLoading(MapManager mapManager) {
        int lastlvl = (int)GameData.data.curLevelUnlocked;
        mapManager.MoveCameraAtLevel(playingNotTheLastLevel ? curLevel : lastlvl);
    }

#endregion
    public bool justUnlockedLevel;

    public void PlayLevel(int lv) {
        curLevel = lv;
        playingNotTheLastLevel = lv != GameData.data.curLevelUnlocked;
        // Find out if we should play the cutscene or not
        currAnimation = GetCutscene(lv);
#if UNITY_EDITOR
        LoadScene(QuScene.GAME);
#else
        if (currAnimation >= 0) {
            LoadScene(QuScene.CUT_GAME);
        } else {
            LoadScene(QuScene.GAME);
        }
#endif
    }

    public void LevelLoaded() {
        if (currentState == QuScene.TUTORIAL) {
            LogHelper.Debug(this, "Setting level as tutorial");
            new GameObject("Tutorial", new System.Type[] { typeof(Tutorial) });
        }
    }

    // Given level index `lv`, returns a non-negative integer if the cutscene with
    // that index should be played, or -1 if none should be played.
    int GetCutscene(int lv) {
        LogHelper.Debug(this, "Called GetCutscene("+lv+")");
        var lvs = GameData.data.levels;
        if (lvs.Count <= lv || levels.Count <= lv)
            return -1;
        var levelPlayerData = lvs[lv];
        var levelData = levels[lv];
        return (levelData.hasCutscene && levelPlayerData.maxScore <= 0) ? levelData.cutscene : -1;
    }

    public void ShowUnlockedCardsThenGoTo(QuScene nextScene) {
        if (Unlock.HasUnlockedCardsToShow()) {
            Unlock.SetUnlockedCardToBeShownAndNextScene(nextScene);
            LoadScene(QuScene.UNLOCK);
        } else {
            LoadScene(nextScene);
        }
    }
}
