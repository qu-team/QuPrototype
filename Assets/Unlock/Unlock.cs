using UnityEngine;
using UnityEngine.UI;

public class Unlock : MonoBehaviour {

    SpriteRenderer card;
    ParticleSystem particles;
    static uint cardToShow;
    static QuScene pendingScene;


    void Awake() {
        card = GameObject.Find("Card").GetComponent<SpriteRenderer>();
        particles = GameObject.FindObjectOfType<ParticleSystem>();
        particles.Stop();
        var cardData = Card.Collection[cardToShow];
        var cardTitle = GameObject.Find("CardTitle").GetComponent<Text>();
        cardTitle.text = "" + cardData.CardNumber + " - " + L10N.Translate(cardData.Name);
    }

    void Start() {
        particles.Play();
    }

    public static bool HasUnlockedCardsToShow() {
        var unlocked = GameData.data.cardsUnlocked;
        for (uint i = 0; i < unlocked.Length; ++i) {
            if (unlocked[i]) continue;
            if (UnlockConditions.IsUnlockConditionTrue(i)) { return true; }
        }
        return false;
    }

    public static void SetUnlockedCardToBeShownAndNextScene(QuScene nextScene) {
        pendingScene = nextScene;
        var unlocked = GameData.data.cardsUnlocked;
        for (uint i = 0; i < unlocked.Length; ++i) {
            if (unlocked[i]) continue;
            if (UnlockConditions.IsUnlockConditionTrue(i)) {
                cardToShow = i;
                unlocked[i] = true;
                GameData.Save();
                return;
            }
        }
    }

    public void Continue() {
        GameManager.Instance.ShowUnlockedCardsThenGoTo(pendingScene);
    }
}
