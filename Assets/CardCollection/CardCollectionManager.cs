using UnityEngine;
using UnityEngine.UI;
using Gestures;
using System.Collections;

public class CardCollectionManager : MonoBehaviour {
	//Editor stuff
	public GesturesDispatcher Dispatcher;
	public Text CardsUnlocked;
	public GameObject cardDetail;
	public GameObject cardList;
	public GameObject Content;
	public GameObject miniCardTemplate;

	public GameObject cardDisplayer;
	
	private int currentDisplayedCard=-1;
	private Animator animator;
	private bool animating=false;
	private bool inDetail=false;

	// Use this for initialization
	void Start () {
		Dispatcher.OnSwipeEnd+=OnSwipeEnd;
		cardDisplayer.GetComponent<CardBehaviour>().GameManager = GameManager.Instance;
		animator = cardDetail.GetComponent<Animator>();
		cardDetail.GetComponent<AnimationCallbacks>().cardManager = this;	
		//CardsUnlocked.text = "Unlocked Cards "+gameManager.UnlockedCards()+"/"+(Card.Collection.Length-1);
		ShowcardList();
		GameManager.Instance.CardCollectionLoaded(this);
	}

	public void ShowCard(int card){
		Debug.Log("showing card ..");
		inDetail = true;
		cardDetail.SetActive(true);
		cardList.SetActive(false);
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

	public void ShowcardList(){
		foreach(Card c in Card.Collection){
			GameObject go = GameObject.Instantiate(miniCardTemplate);
			go.transform.SetParent( Content.transform);
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			ListCardBehaviour bh = miniCardTemplate.GetComponent<ListCardBehaviour>();
			bh.myCard = c;
			bh.manager = this;
			bh.ShowCard();
		}
	}
	public void BackButtonCallback(){
		if(inDetail){
			cardDetail.SetActive(false);
			cardList.SetActive(true);
		}else{
			//TODO LOAD PREV SCENE
		}
	}
	//FIXME DO NOT USE, ONLY FOR ANIMATION EVENT
	public void DisplayDelayedCard(){
		Debug.Log("Card number is "+currentDisplayedCard);
		cardDisplayer.GetComponent<CardBehaviour>().DisplayedCard = Card.Collection[currentDisplayedCard];
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
