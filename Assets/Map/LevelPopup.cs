using UnityEngine;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour{
	public Text Info;
	public Text Title;

    public GameObject playButton;
    public Image star1;
    public Image star2;
    public Image star3;

    int level;
	GameManager gm;

    void Awake() {

    }

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
		this.level = level;
        gameObject.SetActive(true);
        DisablePlayButtonIfLevelLocked();
        DisplayStars();
    }

    void DisablePlayButtonIfLevelLocked() {
        var unlocked = level <= GameData.data.curLevelUnlocked;
        playButton.SetActive(unlocked);
    }

    void DisplayStars() {
        if (GameData.data.levels == null || GameData.data.levels.Count <= level) {
            star1.color = Color.gray;
            star2.color = Color.gray;
            star3.color = Color.gray;
        } else {
            var maxScore = GameData.data.levels[level].maxScore;
            var starScores = GameManager.Instance.Levels[level].stars;
            star1.color = (starScores.first <= maxScore) ? Color.yellow : Color.gray;
            star2.color = (starScores.second <= maxScore) ? Color.yellow : Color.gray;
            star3.color = (starScores.third <= maxScore) ? Color.yellow : Color.gray;
        }
    }

	public void PlayLevel() {
		gm.PlayLevel(level);
	}
}
