using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

	GameManager gameManager;
	GameObject canvas;
	GameObject currAnimation;
	int currAnimationIndex;

	public GameObject[] animations;
	// Use this for initialization
	void Start () {
		gameManager = ((GameObject)GameObject.Find ("GameManager")).GetComponent<GameManager>();
		canvas = GameObject.Find ("Canvas");
		gameManager.AnimationFinishedLoading (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AnimationEnd(){
		Destroy (currAnimation);
		gameManager.AnimationFinished ();
		Debug.Log ("Animation finished");
	}

	public void PlayAnimation(int number){
		GameObject anim = GameObject.Instantiate (animations [number]);
		anim.transform.SetParent (canvas.transform);
		anim.transform.position = Vector2.zero;
		RectTransform trans = anim.GetComponent<RectTransform> ();
		trans.offsetMax = Vector2.zero;
		trans.offsetMin = Vector2.zero;
		trans.localScale = Vector3.one;
		anim.SetActive (true);
		anim.GetComponent<EventAnimationEnd> ().EndAnimation += AnimationEnd;
		currAnimation = anim;
		currAnimationIndex = number;
	}



	public void NextAnimation(){
		if(currAnimationIndex+1 <animations.Length && currAnimationIndex+1 < PlayerPrefs.GetInt("LEVEL_UNLOCKED")){
		PlayAnimation (currAnimationIndex + 1);
		}
	}
	public void Back(){
		gameManager.Back();
	}
}
