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

	private Card _displayedCard;
	public Card DisplayedCard{
		get {
			return _displayedCard;
		}
		set{
			_displayedCard = value;
			title.text = _displayedCard.Name;
			number.text = ""+_displayedCard.CardNumber;
			if(GameManager.IsCardUnlocked( _displayedCard.CardNumber )){
				description.text = _displayedCard.Description;
				cardImage.texture = _displayedCard.CardImage;
			}else{
				cardImage.texture = lockedTexture;
				description.text = _displayedCard.UnlockCondition;	
				}
		}
	}

	void Start(){
	}
}
