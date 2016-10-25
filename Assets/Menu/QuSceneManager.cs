using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


//DO NOT,EVER, INSTANTIATE A GENERIC SCENE MANAGER
public class QuSceneManager : MonoBehaviour {
	public readonly string MySceneName = "Menu";
	public SceneManagerType nextSceneManager;
	public SceneManagerType prevSceneManager;
	public const SceneManagerType MyType = SceneManagerType.MENU;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
		gameObject.tag = "SceneManager";	
	}

	void _nextScene(SceneManagerType manager){
		GameObject sc = (GameObject) Instantiate( SceneManagerTypeDictionay.Managers[nextSceneManager]);
		sc.GetComponent<QuSceneManager>().prevSceneManager = MyType;
		SceneManager.LoadScene( sc.GetComponent<QuSceneManager>().MySceneName );
		Destroy(gameObject);
	}

	void _prevScene(SceneManagerType manager){
		GameObject sc = (GameObject) Instantiate( SceneManagerTypeDictionay.Managers[prevSceneManager]);
		SceneManager.LoadScene( sc.GetComponent<QuSceneManager>().MySceneName );
		Destroy(gameObject);
	}
}

public enum SceneManagerType{
	MENU,
	MOVIE,
	GAME,
}

public class SceneManagerTypeDictionay{
	public static Dictionary<SceneManagerType,GameObject> Managers	= new Dictionary<SceneManager,GameObject>{
		{SceneManagerType.MENU, (GameObject)Resources.Load("MenuManager")},
		{SceneManagerType.MOVIE, (GameObject)Resources.Load("MovieManager")},
		{SceneManagerType.GAME, (GameObject)Resources.Load("GameManager")}
	};
}
