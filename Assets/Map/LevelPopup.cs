using UnityEngine;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour{
	public Text Info;
	public Text Title;
	public Sprite[] Stars;

	public void ShowPopup(GameManager gm, int level){
		Title.text = (level+1)+ " - " + gm.Levels[level].name;	
		Info.text = "Best Score:\n"+ "-"
			+ "\nSaved Qus:\n"+ "-"
			+ "\nTo next level:\n"+ "-";
		//TODO stars
		gameObject.SetActive(true);
	}
}
