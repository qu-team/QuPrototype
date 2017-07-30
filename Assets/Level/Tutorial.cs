using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {

	bool listening;
	Level level;
	GameObject hand;
	GameObject arrow;
	GameObject tutorialText;
	GameObject dim;

	L10N.Label[] labels = new L10N.Label[] {
		L10N.Label.TUTORIAL_1,
		L10N.Label.TUTORIAL_2,
	};
	int curLabel = 0;

	void Awake() {
		hand = GameObject.Find("Hand");
		arrow = GameObject.Find("Arrow");
		dim = GameObject.Find("Dim");
		hand.SetActive(false);
		arrow.SetActive(false);
		level = GameObject.FindObjectOfType<Level>();
		tutorialText = GameObject.Find("TutorialPopup");
		tutorialText.GetComponentInChildren<Text>().text = L10N.Translate(labels[curLabel]);
		level.IsTutorial = true;
		listening = true;

		DisableNotNeededGui();
		InjectTutorialAnimationTrigger();
	}

	void DisableNotNeededGui() {
		GameObject.Find("ScorePanel").SetActive(false);
		GameObject.Find("Quit").SetActive(false);
		GameObject.Find("SavedNumber").SetActive(false);
		GameObject.Find("SavedLabel").SetActive(false);
	}

	void InjectTutorialAnimationTrigger() {
		level.shutter.OnColorSelected -= level.MatchQuColor;
		level.shutter.OnColorSelected += Continue;
	}

	void Start(){
		StartCoroutine(teachColors());
	}

	void Update() {
	}

	void Continue(Color color) {
		if (listening) { return; }
		if( !canMakeMistake && Vector4.Distance( color, level.CurrentQuColor() ) > 0.001f){
			return;
		}else{
			colorTimes++;
		}
		level.Resume();
		level.MatchQuColor(color);
		listening = true;
		hand.SetActive(false);
		dim.SetActive(true);
	}

	public void tutPopupCb(){
		if(clickedColorPop){
			gotTime();
		}else{
			gotColors();
		}
	}

	void gotColors(){
		clickedColorPop = true;
		level.Resume();
		tutorialText.SetActive(false);
	}
	void gotTime(){
		tutorialText.SetActive(false);
		level.Resume();
		clickedTimePop = true;
	}

	private bool clickedColorPop = false;
	private bool clickedTimePop = false;

	private int colorTimes;
	IEnumerator teachColors(){
		level.Pause();
		colorTimes = 0;
		while (colorTimes < 3){
			if (!level.Paused && listening && level.shutter.opening <= 0.1f) {
				level.Pause();
				StartCoroutine(ShowColorEquality());

			}
			yield return null;
		}
		StartCoroutine(teachTime());	
	}

	bool showedTime;
	IEnumerator teachTime(){
		level.Pause();
		tutorialText.GetComponentInChildren<Text>().text = L10N.Translate(labels[++curLabel]);
		tutorialText.SetActive(true);
		while(!clickedTimePop){
			yield return null;
		}
		colorTimes =0;
		showedTime = false;
		while(colorTimes<1){
			if(level.shutter.opening > 0.1f){
				showedTime = false;
			}
			if (!showedTime && !level.Paused && listening && level.shutter.opening <= 0.0001f ) {
				level.Pause();
				dim.SetActive(false);
				StartCoroutine(showTime());
			}
			yield return null;
		}
		canMakeMistake = true;
		StartCoroutine( pleaseFixIt());
		listening = false;
	}

	bool canMakeMistake = false;
	IEnumerator showTime(){
		UnityEngine.UI.Image rend = GameObject.FindObjectOfType<DeathRing>()
			.GetComponent<UnityEngine.UI.Image>();
		float time =0f;
		int pulses=0;
		while(pulses<3){
			LogHelper.Debug("Time", "pulsing "+ pulses+"time");
			while(time < 0.4f){
				time += Time.deltaTime;
				rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, time/0.4f);
				yield return null;
			}
			time =0;
			while(time < 0.4f){
				time += Time.deltaTime;
				rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 1f-time/0.4f);
				yield return null;
			}
			time =0;
			pulses++;
		}
		listening = false;
		dim.SetActive(false);
		rend.color = Color.white;
		colorTimes++;
		level.Resume();
		showedTime = true;
	}

	// Every day, we stray further from God.
	IEnumerator pleaseFixIt(){
		while(true){
			listening = false;
			dim.SetActive(false);
			yield return null;
		}
	}
	
	IEnumerator ShowColorEquality() {
		//SetArrowAtCorrectColor();
		//arrow.GetComponent<SpriteRenderer>().color = HalfColor(level.qu.Color);
		//arrow.SetActive(true);
		//yield return new WaitForSeconds(1f);
		//arrow.SetActive(false);
		while(!clickedColorPop) yield return null;
		SetHandAtCorrectColor();
		hand.GetComponent<SpriteRenderer>().color = HalfColor(level.qu.Color);
		hand.SetActive(true);
		yield return null;
		listening = false;
		dim.SetActive(false);
	}

	Color HalfColor(Color color) {
		return new Color(color.r / 2, color.g / 2, color.b / 2);
	}

	// Warning: this only works if there are exactly 3 blades!
	void SetArrowAtCorrectColor() {
		foreach (var blade in level.shutter.blades) {
			if (blade.GetComponentInChildren<MeshRenderer>().material.color == level.qu.Color) {
				var bpos = blade.transform.Find("Shape").position;
				var qpos = level.qu.transform.position;
				float coef = bpos.y > qpos.y
					? 0.7f // Upper blade
					: bpos.x > qpos.x
					? 0.7f // Lower blade
					: 0.7f // Left blade
					;
				arrow.transform.position = bpos + (qpos - bpos) * coef + new Vector3(0, 0, -3);
				float angle = bpos.y > qpos.y
					? 15f // Upper blade
					: bpos.x > qpos.x
					? 80f // Lower blade
					: 130f // Left blade
					;
				arrow.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
				return;
			}
		}
	}

	// Warning: this only works if there are exactly 3 blades!
	void SetHandAtCorrectColor() {
		foreach (var blade in level.shutter.blades) {
			if (blade.GetComponentInChildren<MeshRenderer>().material.color == level.qu.Color) {
				var bpos = blade.transform.Find("Shape").position;
				var qpos = level.qu.transform.position;
				float coef = bpos.y > qpos.y
					? 0.5f // Upper blade
					: bpos.x > qpos.x
					? 0.5f // Lower blade
					: 0.5f // Left blade
					;
				hand.transform.position = bpos + (qpos - bpos) * coef + new Vector3(0.5f, -1f, -3f);
				return;
			}
		}
	}
}
