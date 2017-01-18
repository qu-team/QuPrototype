using UnityEngine;
using UnityEngine.UI;

public class ListCardBehaviour : MonoBehaviour{
	public CardCollectionManager manager;
	public Text myText;
	public RawImage myImg;
	public Card myCard;

	public void ShowCard(){
		if(GameData.data.IsCardUnlocked(myCard.CardNumber)){
			myImg.texture = myCard.CardImage;
			myText.text = myCard.CardNumber+"";
		}
		else ShowLocked();

	}
	public void ShowLocked(){
		myText.text = myCard.CardNumber+""; 
	}
	public void ShowDetailCard(){
		manager.ShowCard(int.Parse(myText.text)-1);
	}
}
