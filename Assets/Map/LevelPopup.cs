using UnityEngine;
using UnityEngine.UI;

public class LevelPopup : MonoBehaviour {
    public Text Info;
    public Text Title;
    public Text lockedText;

    public GameObject playButton;
    public GameObject lockedPanel;
    public GameObject statsPanel;
    public GameObject buttonPanel;
    public GameObject loadingPanel;
    public Image star1;
    public Image star2;
    public Image star3;

    int level;
    GameManager gm;

    void Awake() {

    }

    public void ShowPopup(GameManager gm, int level, Color color){
        if (level >= gm.Levels.Count) {
            LogHelper.Warn(this, "Called ShowPopup(lv=" + (level + 1) + "), but only "
                    + gm.Levels.Count + " levels have been loaded.");
            return;
        }
        this.gm = gm;
        Title.text = (level + 1) + " - " + gm.Levels[level].name;
        LevelSaveData? lvdata = null;
        Debug.Assert(GameData.data.levels != null && GameData.data.levels.Count > level);
        lvdata = GameData.data.levels[level];

        Info.text = L10N.Translate(L10N.Label.BEST_SCORE) + ":\n"
                + (lvdata.HasValue ? lvdata.Value.maxScore.ToString() : "-")
                + "\n" + L10N.Translate(L10N.Label.SAVED_QUS) + ":\n" 
                + (lvdata.HasValue ? lvdata.Value.quSaved.ToString() : "-");
		if(level <9){
        	Info.text  += "\n" + L10N.Translate(L10N.Label.TO_NEXT_LEVEL) + ":\n" 
                + gm.Levels[level].quToNextLevel.ToString();
		}

        this.level = level;
        gameObject.SetActive(true);
        GetComponent<Image>().color = color;
        DisablePlayButtonIfLevelLocked();
        DisplayStars();
    }

    void DisablePlayButtonIfLevelLocked() {
        var unlocked = level <= GameData.data.curLevelUnlocked;
        playButton.SetActive(unlocked);
        lockedPanel.SetActive(!unlocked);
        if (!unlocked) {
            var missingQu = GameManager.Instance.Levels[level-1].quToNextLevel - GameData.data.levels[level-1].quSaved;
            lockedText.text = lockedText.text.Replace("$$", missingQu.ToString());
        }
        statsPanel.SetActive(unlocked);
    }

    void DisplayStars() {
        Debug.Assert(GameData.data.levels != null && GameData.data.levels.Count > level);
        var maxScore = GameData.data.levels[level].maxScore;
        var starScores = GameManager.Instance.Levels[level].stars;
        star1.color = (starScores.first <= maxScore) ? Color.yellow : Color.gray;
        star2.color = (starScores.second <= maxScore) ? Color.yellow : Color.gray;
        star3.color = (starScores.third <= maxScore) ? Color.yellow : Color.gray;
    }

    public void PlayLevel() {
        buttonPanel.SetActive(false);
        loadingPanel.SetActive(true);
        gm.PlayLevel(level);
    }
}
