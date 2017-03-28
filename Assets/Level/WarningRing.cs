using UnityEngine;

public class WarningRing : MonoBehaviour{
	public void ResumeLevel(){
		GameObject.FindObjectOfType<Level>().GetComponent<Level>().ShowedWarningRing();
	}
}
