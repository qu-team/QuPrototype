using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class GameManager : MonoBehaviour {

	enum GameState {
		MAIN_MENU,
		CARD_COLLECTION,
		COLLECTION_CUT,
		GAME_CUT,
		GAME,
		MAP,
	}

	public static GameManager Instance {
		get;
		private set;
	}

	private GameState currentState;
	private LevelsData levels;

	//Animation screen vars
	public int currAnimation = 0;

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
		//currentState = GameState.MAIN_MENU;
		//FIXME DEBUG
		currentState = GameState.COLLECTION_CUT;
		PlayerPrefs.SetInt("LEVEL_UNLOCKED", 10);
		levels = new LevelsData(Application.dataPath + Path.DirectorySeparatorChar + "levels.json");	
	}

	// Use this for initialization
	void Start() {
		// Load player stats
		if (!GameData.Load())
		    LogHelper.Warn(this, "Game data was not loaded from save file.");
	}
	
	// Update is called once per frame
	void Update() {
	
	}

	public void Back() {
		//TODO caricare la scena precedente
	}

	public void AnimationFinishedLoading(AnimationController controller) {
		controller.PlayAnimation(currAnimation);
	}

	public void AnimationFinished(AnimationController currAnimController) {
		switch (currentState) {
		case GameState.COLLECTION_CUT:
			currAnimController.NextAnimation();
			break;
		default:
			//LoadNextScene();
			UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Level");
			break;
		}
	}

	void LoadNextScene() {

	}

#region CardCollection
	public bool IsCardUnlocked(int number) {
		//FIXME
		return (number <= Card.Collection.Length) && number%2 == 0;
	}

	public void CardCollectionLoaded(CardCollectionManager manager) {
		//FIXME
		manager.ShowCardList();
	}

	public int UnlockedCards() {
		//TODO
		return 1;
	}
#endregion

#region Map
	public void MapFinishedLoading(MapManager mapManager) {
		//Todo
		int lastlvl=0;
		mapManager.MoveCameraAtLevel(lastlvl);
	}

#endregion

	public void PlayLevel(int lv) {
		curLevel = lv;
		// Find out if we should play the cutscene or not
		if (GameData.data.levels == null || GameData.data.levels.Count <= lv 
				|| GameData.data.levels[lv].maxScore <= 0) 
		{
			currAnimation = lv + 1;
			currentState = GameState.GAME_CUT;
			SceneManager.LoadScene("Animations");
		} else {
			SceneManager.LoadScene("Level");
		}
	}
}
