using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private GameState currentState;

	//Animation screen vars
	public int currAnimation=0;
	private AnimationController currAnimController;


	void Awake(){
		DontDestroyOnLoad (gameObject);
		//currentState = GameState.MAIN_MENU;
		//FIXME DEBUG
		currentState = GameState.COLLECTION_CUT;
		PlayerPrefs.SetInt ("LEVEL_UNLOCKED", 10);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Back(){
		//TODO caricare la scena precedente
	}

	public void AnimationFinishedLoading(AnimationController controller){
		controller.PlayAnimation (currAnimation);
	}

	public void AnimationFinished(AnimationController currAnimController){
		switch (currentState) {
		case GameState.COLLECTION_CUT:
			currAnimController.NextAnimation ();
			break;
		default:
			//LoadNextScene();
			UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Level");
			break;
		}
	}

	void LoadNextScene(){

	}

#region CardCollection

	public bool IsCardUnlocked(int number){
		//FIXME
		return (number <= Card.Collection.Length) && number%2==0;
	}

	public void CardColletionLoaded(CardCollectionManager manager){
		//FIXME
		manager.ShowCard(1);
	}

	public int UnlockedCards(){
		//TODO
		return 1;
	}
#endregion

	enum GameState{
		MAIN_MENU,
		CARD_COLLECTION,
		COLLECTION_CUT,
		GAME_CUT,
		GAME,
		MAP,
	}
}
