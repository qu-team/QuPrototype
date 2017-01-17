using UnityEngine;
using Gestures;
using System.Collections;

public class AnimationController : MonoBehaviour {

    GameManager gameManager;
    GameObject canvas;
    GameObject currAnimation;
    public Transform AnimationRoot;
    public GameObject animationControls;
    int currAnimationIndex;
    public GesturesDispatcher dispatcher;
    public bool IngameCut{
        get;
        set;
    }
    private bool disabling;

    public GameObject[] animations;
    // Use this for initialization
    void Start() {
        gameManager = GameManager.Instance;
        canvas = GameObject.Find("Canvas");
        gameManager.AnimationFinishedLoading(this);
        dispatcher.OnTapEnd += OnTapEnd;
    }

    void AnimationEnd(){
        Destroy(currAnimation);
        gameManager.AnimationFinished(this);
        LogHelper.Debug(this, "Animation finished");
    }

    public void PauseCurrentAnimation(){
        if(currAnimation == null) return;
        foreach(Animation anim in currAnimation.GetComponentsInChildren<Animation>(true)){
            anim.enabled = false;
        }
    }
    public void ResumeCurrentAnimation(){
        HideControls();
        if(currAnimation == null) return;
        foreach(Animation anim in currAnimation.GetComponentsInChildren<Animation>(true)){
            anim.enabled = true;
        }
    }
    private void HideControls(){
        animationControls.SetActive(false);
        disabling = true;
    }
    public void PlayAnimation(int number){
        Destroy(currAnimation);
        GameObject anim = GameObject.Instantiate(animations [number]);
        anim.transform.SetParent(AnimationRoot);
        anim.transform.position = Vector2.zero;
        RectTransform trans = anim.GetComponent<RectTransform>();
        trans.offsetMax = Vector2.zero;
        trans.offsetMin = Vector2.zero;
        trans.localScale = Vector3.one;
        anim.SetActive(true);
        anim.GetComponent<EventAnimationEnd>().EndAnimation += AnimationEnd;
        currAnimation = anim;
        currAnimationIndex = number;
    }

    public void PrevAnimation(){
        if(currAnimationIndex > 0){
            HideControls();
            PlayAnimation(currAnimationIndex -1);
        }
    }
    public void NextAnimation(){
        HideControls();
        if(currAnimationIndex+1 <animations.Length && currAnimationIndex+1 < PlayerPrefs.GetInt("LEVEL_UNLOCKED")){
            PlayAnimation(currAnimationIndex + 1);
        } else {
            gameManager.Back();
        }
    }
    public void Back(){
        gameManager.Back();
    }

    private void OnTapEnd(Tap tap){
        if(disabling) {
            disabling = false;
            return;
        }
        if(IngameCut) return;
        if(animationControls.active == false){
            animationControls.SetActive(true);
            PauseCurrentAnimation();
        }
    }
}
