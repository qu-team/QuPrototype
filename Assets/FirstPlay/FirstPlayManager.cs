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

	public void LanguageCallback(int lang){
		LogHelper.Debug("Buttons","Called change lang " + lang);
		switch(lang){
		case 0:
			L10N.CurrentLanguage = SystemLanguage.English;
			break;
		case 1:
			L10N.CurrentLanguage = SystemLanguage.Italian;
			break;
		}
		gameObject.AddComponent<L10N>();
		GetComponent<Animator>().SetTrigger( "LanguageChosen" );
	}

	public void FirstAnimationCallback(){
		GameManager.Instance.LoadScene(QuScene.TUTORIAL);
	}
}
