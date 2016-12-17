using UnityEngine;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour{
	public Text Info;
	public Text Title;
	public Sprite[] Stars;

	int level;

	GameManager gm;

	public void ShowPopup(GameManager gm, int level){
		if (level >= gm.Levels.Count) {
			LogHelper.Warn(this, "Called ShowPopup(lv=" + level + "), but only " + gm.Levels.Count
					+ " levels have been loaded.");
			return;
		}
		this.gm = gm;
		print("level = " + level);
		Title.text = (level+1)+ " - " + gm.Levels[level].name;	
		Info.text = "Best Score:\n"+ "-"
			+ "\nSaved Qus:\n"+ "-"
			+ "\nTo next level:\n"+ "-";
		//TODO stars
		this.level = level;
		gameObject.SetActive(true);
	}

	public void PlayLevel() {
		gm.PlayLevel(level);
	}
}
