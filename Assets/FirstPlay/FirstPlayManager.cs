using UnityEngine;

public class FirstPlayManager : MonoBehaviour {

	public bool editorDebug;
	void Start(){
		if(editorDebug){
			GameManager.Instance.LoadScene(QuScene.TUTORIAL);
			return;
		}
		if(PlayerPrefs.GetInt(Preferences.PLAYED_TUTORIAL,0) != 0){
			UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
		}
	}

	public void FirstAnimationCallback(){
		GameManager.Instance.LoadScene(QuScene.TUTORIAL);
	}
}
