using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour {

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
		{QuScene.SETTINGS, "Preferences"},
		{QuScene.SHARE, "ShareScore"}

	};

    //Animation screen vars
    public int currAnimation = 0;
    public bool ignoreNextEscape = false;

    private AnimationController currAnimController;
    private int curLevel;

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
        //currentState = QuScene.MENU;
        //FIXME DEBUG
        currentState = QuScene.CUT_COLLECTION;
        PlayerPrefs.SetInt("LEVEL_UNLOCKED", 10);
        levels = new LevelsData(Application.dataPath + Path.DirectorySeparatorChar + "levels.json");
    }

    void Start() {
        // Load player stats
        if (!GameData.Load())
            LogHelper.Warn(this, "Game data was not loaded from save file.");
    }

    void Update() {
        if (currentState != QuScene.CARD_COLLECTION && Input.GetKeyUp(KeyCode.Escape)) {
            Back();
        }
    }

    public void Back() {
        switch (currentState) {
        case QuScene.CARD_COLLECTION:
        case QuScene.CUT_COLLECTION:
        case QuScene.MAP:
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

    public void AnimationFinishedLoading(AnimationController controller) {
        controller.IngameCut = currentState == QuScene.CUT_GAME;
        controller.PlayAnimation(currAnimation);
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

	public void LoadScene(QuScene scene){
		switch(scene){
			case QuScene.GAME:
				SceneManager.LoadSceneAsync(sceneNames[scene]);
				break;
			default:
				SceneManager.LoadScene(sceneNames[scene]);
                break;
		}
		currentState = scene;
	}

#region CardCollection

    public void CardCollectionLoaded(CardCollectionManager manager) {
        manager.ShowCardList();
    }

#endregion

#region Map
    public void MapFinishedLoading(MapManager mapManager) {
        int lastlvl = (int)GameData.data.curLevelUnlocked; 
        mapManager.MoveCameraAtLevel(lastlvl);
    }

#endregion

    public void PlayLevel(int lv) {
        curLevel = lv;
        // Find out if we should play the cutscene or not
        if (GameData.data.levels == null || GameData.data.levels.Count <= lv 
                || GameData.data.levels[lv].maxScore <= 0) 
        {
            currAnimation = lv ;
            LoadScene(QuScene.CUT_GAME);
        } else {
            LoadScene(QuScene.GAME);
        }
    }

}

public enum QuScene{
	MENU,
	GAME,
	CARD_COLLECTION,
	CUT_COLLECTION,
	CUT_GAME,
	MAP,
	SETTINGS,
	SHARE
}
