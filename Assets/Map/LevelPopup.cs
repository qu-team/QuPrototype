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
		LevelSaveData? lvdata = null;
		if (GameData.data.levels != null && GameData.data.levels.Count > level)
			lvdata = GameData.data.levels[level];
		Info.text = L10N.Translate(L10N.Label.BEST_SCORE) + ":\n"+ (lvdata.HasValue ? lvdata.Value.maxScore.ToString() : "-")
			+ "\n" + L10N.Translate(L10N.Label.SAVED_QUS) + ":\n" + (lvdata.HasValue ? lvdata.Value.quSaved.ToString() : "-")
			+ "\n" + L10N.Translate(L10N.Label.TO_NEXT_LEVEL) + ":\n" + gm.Levels[level].quToNextLevel.ToString();
		//TODO stars
		this.level = level;
        gameObject.SetActive(true);
        DisablePlayButtonIfLevelLocked();
    }

    void DisablePlayButtonIfLevelLocked() {
        var unlocked = level <= GameData.data.curLevelUnlocked;
        GameObject.Find("PlayButton").GetComponent<Button>().interactable = unlocked;
    }

	public void PlayLevel() {
		gm.PlayLevel(level);
	}
}
