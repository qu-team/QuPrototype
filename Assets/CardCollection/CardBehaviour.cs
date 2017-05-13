using UnityEngine;
using UnityEngine.UI;

class CardBehaviour : MonoBehaviour {

#pragma warning disable 0649
    //Editor stuff
    public CardCollectionManager manager;
    public Text title;
    public Text description;
    public RawImage cardImage;
    public Text number;
    public Texture lockedTexture;
#pragma warning restore 0649

    private Card displayedCard;
    public Card DisplayedCard{
        get {
            return displayedCard;
        }
        set {
            displayedCard = value;
            title.text = L10N.Translate(displayedCard.Name);
            number.text = "" + displayedCard.CardNumber;
            if (GameData.data.IsCardUnlocked(displayedCard.CardNumber)) {
                description.text = L10N.Translate(displayedCard.Description);
                cardImage.texture = displayedCard.CardImage;
            } else {
                cardImage.texture = lockedTexture;
                description.text = L10N.Translate(displayedCard.UnlockCondition);
            }
        }
    }
}
