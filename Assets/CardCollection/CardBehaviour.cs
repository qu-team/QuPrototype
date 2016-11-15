using UnityEngine;
using UnityEngine.UI;

class CardBehaviour : MonoBehaviour{

	//Editor stuff
	public CardCollectionManager manager;
	public Text Title;
	public Text Description;
	public RawImage CardImage;
	public Text Number;


	public GameManager gm;

	private Card _displayedCard;
	public Card DisplayedCard{
		get {
			return _displayedCard;
		}
		set{
			_displayedCard = value;
			Title.text = _displayedCard.Name;
			Number.text = ""+_displayedCard.CardNumber;
			if(gm.IsCardUnlocked( _displayedCard.CardNumber )){
				Description.text = _displayedCard.Description;
				//FIXME, need obj pool
				Debug.Log("Cards"+System.IO.Path.DirectorySeparatorChar+_displayedCard.CardNumber);
				CardImage.texture = Resources.Load("Cards"+System.IO.Path.DirectorySeparatorChar+
						_displayedCard.CardNumber) as Texture;
			}else{
				
				}
		}
	}

	void Start(){
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
}
