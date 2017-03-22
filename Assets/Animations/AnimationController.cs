using UnityEngine;
using UnityEngine.UI;
using Gestures;

public class AnimationController : MonoBehaviour {

    GameManager gameManager;
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
    private bool playingLast;

    public GameObject[] animations;
    // Use this for initialization
    void Start() {
        gameManager = GameManager.Instance;
        dispatcher.OnTapEnd += OnTapEnd;
        gameManager.AnimationFinishedLoading(this);
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
        playingLast = false;
        animationControls.SetActive(false);
        disabling = true;
    }
    public void PlayAnimation(int number){
        if (number < 0 || number >= animations.Length) {
            playingLast = false;
            AnimationEnd();
            return;
        }
        Destroy(currAnimation);
        GameObject anim = GameObject.Instantiate(animations[number]);
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
        var message = anim.transform.FindChild("Text").GetComponent<Text>();
        message.text = L10N.Translate(message.text);
    }

    public void PrevAnimation(){
            if(playingLast){
                PlayAnimation( currAnimationIndex);
                HideControls();
                return;
            }
            HideControls();
            PlayAnimation(Mathf.Max(currAnimationIndex-1,0));
    }
    public void NextAnimation(){
        HideControls();
        if(currAnimationIndex+1 <animations.Length && currAnimationIndex+1 < (int)GameData.data.CurAnimationUnlocked) {
            PlayAnimation(currAnimationIndex + 1);
        } else {
            playingLast = true;
            ShowAnimationControls();
        }
    }
    public void Back(){
        gameManager.Back();
    }
    private void ShowAnimationControls(){
        animationControls.SetActive(true);
        LogHelper.Debug("Animations", "Current index is "+currAnimationIndex);
        GameObject.Find("Next").GetComponent<Button>().interactable = !(currAnimationIndex+1 >= animations.Length 
                || currAnimationIndex+1 >= (int) GameData.data.curLevelUnlocked);
        //GameObject.Find("Prev").GetComponent<Button>().interactable = !(currAnimationIndex < 1);
        PauseCurrentAnimation();
    }
    private void OnTapEnd(Tap tap){
        if(disabling) {
            disabling = false;
            return;
        }
        if (IngameCut) return;
        if (!animationControls.activeSelf) {
            ShowAnimationControls();
        }
    }
}
