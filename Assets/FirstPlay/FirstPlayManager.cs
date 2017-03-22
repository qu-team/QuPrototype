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

	public void languageCallback(int lang){
		LogHelper.Debug("Buttons","Called change lang");
		switch(lang){
			case 1:
				L10N.CurrentLanguage = SystemLanguage.Italian;
				break;
			case 0:
				L10N.CurrentLanguage = SystemLanguage.English;
				break;
		}
		GetComponent<Animator>().SetTrigger( "LanguageChosen" );
	}

	public void FirstAnimationCallback(){
		GameManager.Instance.LoadScene(QuScene.TUTORIAL);
	}
}
