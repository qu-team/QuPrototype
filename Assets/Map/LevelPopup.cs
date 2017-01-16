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
			LogHelper.Warn(this, "Called ShowPopup(lv=" + (level + 1) + "), but only "
					+ gm.Levels.Count + " levels have been loaded.");
			return;
		}
		this.gm = gm;
		print("level = " + (level + 1));
		Title.text = (level + 1) + " - " + gm.Levels[level].name;	
		LevelData? lvdata = null;
		if (GameData.data.levels != null && GameData.data.levels.Count > level)
			lvdata = GameData.data.levels[level];
		Info.text = "Best Score:\n"+ (lvdata.HasValue ? lvdata.Value.maxScore.ToString() : "-")
			+ "\nSaved Qus:\n"+ (lvdata.HasValue ? lvdata.Value.quSaved.ToString() : "-")
			+ "\nTo next level:\n"+ "-";
		//TODO stars
		this.level = level;
		gameObject.SetActive(true);
	}

	public void PlayLevel() {
		gm.PlayLevel(level);
	}
}
