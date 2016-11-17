using UnityEngine;
using UnityEngine.UI;

class CardBehaviour : MonoBehaviour{

	//Editor stuff
	public CardCollectionManager manager;
	public Text Title;
	public Text Description;
	public RawImage CardImage;
	public Text Number;
	public Texture LockedTexture;

	public GameManager GameManager;

	private Card _displayedCard;
	public Card DisplayedCard{
		get {
			return _displayedCard;
		}
		set{
			_displayedCard = value;
			Title.text = _displayedCard.Name;
			Number.text = ""+_displayedCard.CardNumber;
			if(GameManager.IsCardUnlocked( _displayedCard.CardNumber )){
				Description.text = _displayedCard.Description;
				CardImage.texture = _displayedCard.CardImage;
			}else{
				CardImage.texture = LockedTexture;
				Description.text = _displayedCard.UnlockCondition;	
				}
		}
	}

	void Start(){
	}
}
