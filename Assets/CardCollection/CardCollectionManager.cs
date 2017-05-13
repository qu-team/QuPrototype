using UnityEngine;
using UnityEngine.UI;
using Gestures;
using System.Collections;

public class CardCollectionManager : MonoBehaviour {
    //Editor stuff
    public GesturesDispatcher Dispatcher;
    public Text CardsUnlocked;
    public GameObject cardList;
    public GameObject Content;
    public GameObject miniCardTemplate;

    public GameObject cardDisplayer;
    
    private int currentDisplayedCard=-1;
    private Animator animator;
    private bool animating=false;
    private bool inDetail=false;
    private bool displayingUnlockCondition = false;

    // Use this for initialization
    void Start() {
        Dispatcher.OnSwipeEnd+=OnSwipeEnd;
        animator = cardDisplayer.GetComponent<Animator>();
        cardDisplayer.GetComponent<AnimationCallbacks>().cardManager = this;    
        //CardsUnlocked.text = "Unlocked Cards "+gameManager.UnlockedCards()+"/"+(Card.Collection.Length-1);
        PopulateCardList();    
        GameManager.Instance.CardCollectionLoaded(this);
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            BackButtonCallback();
        }
    }

    public void ShowCardList(){
        cardDisplayer.SetActive(false);
        cardList.SetActive(true);
        currentDisplayedCard = -1;
        inDetail = false;
    }

    public void ShowCard(int card){
        inDetail = true;
        displayingUnlockCondition = false;
        cardDisplayer.SetActive(true);
        cardList.SetActive(false);
        if(card == currentDisplayedCard || card<0 || card>=Card.Collection.Length || animating) return;
        animating = true;
        if(card > currentDisplayedCard){
            animator.SetTrigger("left");
        }
        else{
            animator.SetTrigger("right");
        }
        currentDisplayedCard = card;
    }

    private void PopulateCardList(){
        foreach(Card c in Card.Collection){
            GameObject go = GameObject.Instantiate(miniCardTemplate);
            go.transform.SetParent( Content.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            ListCardBehaviour bh = go.GetComponent<ListCardBehaviour>();
            bh.myCard = c;
            bh.manager = this;
            bh.ShowCard();
        }
    }

    public void BackButtonCallback(){
        if (animating)
            return;

        if(inDetail){
            ShowCardList();
        }else{
            GameManager.Instance.Back();
        }
    }

    public void CardNumberCallback() {
        if (!GameData.data.IsCardUnlocked(currentDisplayedCard + 1))
            return;

        var displayedCard = cardDisplayer.GetComponent<CardBehaviour>();
        if (displayingUnlockCondition) {
            displayedCard.description.text = L10N.Translate(displayedCard.DisplayedCard.Description);
        } else {
            displayedCard.description.text = L10N.Translate(displayedCard.DisplayedCard.UnlockCondition);
        }
        displayingUnlockCondition = !displayingUnlockCondition;
    }

    //FIXME DO NOT USE, ONLY FOR ANIMATION EVENT
    public void DisplayDelayedCard(){
        //LogHelper.Debug(this, "Delayed card index is "+currentDisplayedCard);
        cardDisplayer.GetComponent<CardBehaviour>().DisplayedCard = Card.Collection[currentDisplayedCard];
    }

    public void AnimationEnded(){
        animating = false;
    }

    void OnSwipeEnd(Swipe sw){
        if(!cardDisplayer.activeSelf) return;
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
