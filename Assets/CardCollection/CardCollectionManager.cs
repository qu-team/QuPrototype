using UnityEngine;
using UnityEngine.UI;
using Gestures;
using System.Collections;

public class CardCollectionManager : MonoBehaviour {
	//Editor stuff
	public GesturesDispatcher Dispatcher;
	public Text CardsUnlocked;
	
	GameManager gameManager;
	public GameObject CardDisplayer;
	
	private int currentDisplayedCard=-1;
	private Animator animator;
	private bool animating=false;

	// Use this for initialization
	void Start () {
		Dispatcher.OnSwipeEnd+=OnSwipeEnd;
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		CardDisplayer.GetComponent<CardBehaviour>().GameManager = gameManager;
		animator = GetComponent<Animator>();
		CardsUnlocked.text = "Unlocked Cards "+gameManager.UnlockedCards()+"/"+(Card.Collection.Length-1);
		gameManager.CardColletionLoaded(this);
	}

	public void ShowCard(int card){
		if(card == currentDisplayedCard || card<1 || card>=Card.Collection.Length || animating) return;
		animating = true;
		if(card > currentDisplayedCard){
			animator.SetTrigger("left");
		}
		else{
			animator.SetTrigger("right");
		}
		currentDisplayedCard = card;
	}
	//FIXME DO NOT USE, ONLY FOR ANIMATION EVENT
	public void DisplayDelayedCard(){
		Debug.Log("Card number is "+currentDisplayedCard);
		CardDisplayer.GetComponent<CardBehaviour>().DisplayedCard = Card.Collection[currentDisplayedCard];
	}	
	public void AnimationEnded(){
		animating = false;
	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnSwipeEnd(Swipe sw){
		if(animating)return;
		if(sw.Duration < 0.2f){
			if(Vector2.Dot(sw.End-sw.Start, Vector2.left)<0){
				ShowCard(currentDisplayedCard-1);
			}
			else{
				ShowCard(currentDisplayedCard +1);
			}
		}
	}

}
