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
		LanguageCallback();
	}

	public void LanguageCallback(){

		var lang = Application.systemLanguage;
		LogHelper.Debug("Buttons","Called change lang " + lang);
		switch(lang){
		case SystemLanguage.English:
		case SystemLanguage.Italian:
			L10N.CurrentLanguage = lang;
			break;
		default:
			L10N.CurrentLanguage = SystemLanguage.English;
			break;
		}
		gameObject.AddComponent<L10N>();
		GetComponent<Animator>().SetTrigger( "LanguageChosen" );
	}

	public void FirstAnimationCallback(){
		GameManager.Instance.LoadScene(QuScene.TUTORIAL);
	}
}
