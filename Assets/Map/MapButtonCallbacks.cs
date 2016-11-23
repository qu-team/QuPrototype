using UnityEngine;

public class MapButtonCallbacks:MonoBehaviour{

	public GameObject popup;
	public void BackButtonCallback(){
		popup.SetActive(false);
	}
}
