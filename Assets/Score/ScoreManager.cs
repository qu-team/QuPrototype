using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public GameObject locked;
    public GameObject unlocked;
    public Text unlockedText;
    public Text lockedText1;
    public Text lockedText2;

    void Start(){
        if(GameManager.Instance.justUnlockedLevel){
            unlocked.SetActive(true);
            unlockedText.text = string.Format(
                L10N.Translate( L10N.Label.UNLOCKED_LEVEL),
                (int)GameData.data.curLevelUnlocked +1);
        }else{
            locked.SetActive(true);
            int quSaved =(int) GameData.data.levels[GameManager.Instance.CurrentLevel].quSaved;

            lockedText1.text = string.Format(
                L10N.Translate( L10N.Label.LOCKED_LEVEL1),
                    quSaved);
            lockedText2.text = string.Format(L10N.Translate(L10N.Label.LOCKED_LEVEL2),
                GameManager.Instance.Levels[GameManager.Instance.CurrentLevel].quToNextLevel- quSaved
                );
        }
    }

    public void buttonCallback(){
        if(GameManager.Instance.goToShare){
            GameManager.Instance.LoadScene(QuScene.SHARE);
        }else{
            GameManager.Instance.LoadScene(QuScene.MAP);
        }
    }
}
