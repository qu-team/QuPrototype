using UnityEngine;
using UnityEngine.UI;

class CardBehaviour : MonoBehaviour{

	//Editor stuff
	public CardCollectionManager manager;
	public Text title;
	public Text description;
	public RawImage cardImage;
	public Text number;
	public Texture lockedTexture;

	public GameManager GameManager;

	private Card displayedCard;
	public Card DisplayedCard{
		get {
			return displayedCard;
		}
		set{
			displayedCard = value;
			title.text = displayedCard.Name;
			number.text = ""+displayedCard.CardNumber;
			if(GameData.data.IsCardUnlocked(displayedCard.CardNumber)){
				description.text = displayedCard.Description;
				cardImage.texture = displayedCard.CardImage;
			}else{
				cardImage.texture = lockedTexture;
				description.text = displayedCard.UnlockCondition;	
				}
		}
	}

	void Start(){
	}
}
