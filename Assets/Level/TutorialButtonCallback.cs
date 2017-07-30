using UnityEngine;

public class TutorialButtonCallback:MonoBehaviour{
	
	public void Start(){
		if(!GameObject.FindObjectOfType<Level>().IsTutorial){
			Destroy(gameObject);
		}
	}

	public void cb(){
		GameObject.FindObjectOfType<Tutorial>().tutPopupCb();
	}
}
