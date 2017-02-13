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

    public GameObject[] animations;
    // Use this for initialization
    void Start() {
        gameManager = GameManager.Instance;
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
        if (number >= animations.Length) {
            AnimationEnd();
            return;
        }
        Destroy(currAnimation);
        Debug.Assert(number < animations.Length);
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
            HideControls();
            PlayAnimation(Mathf.Max(currAnimationIndex-1,0));
    }
    public void NextAnimation(){
        HideControls();
        if(currAnimationIndex+1 <animations.Length && currAnimationIndex+1 < (int)GameData.data.curLevelUnlocked){
            PlayAnimation(currAnimationIndex + 1);
        } else {
            //gameManager.Back();
			ShowAnimationControls();
        }
    }
    public void Back(){
        gameManager.Back();
    }
	private void ShowAnimationControls(){
		animationControls.SetActive(true);
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
