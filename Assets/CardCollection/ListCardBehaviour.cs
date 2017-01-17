using UnityEngine;
using UnityEngine.UI;

public class ListCardBehaviour : MonoBehaviour{
	public CardCollectionManager manager;
	public Text myText;
	public RawImage myImg;
	public Card myCard;

	public void ShowCard(){
		myImg.texture = myCard.CardImage;
		myText.text = myCard.CardNumber+"";

	}
	public void ShowLocked(){
		myText.text = myCard.CardNumber+""; 
	}
	public void ShowDetailCard(){
		manager.ShowCard(int.Parse(myText.text)-1);
	}
}
