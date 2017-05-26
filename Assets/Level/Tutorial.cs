using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	bool listening;
	Level level;
	GameObject hand;
	GameObject arrow;


	void Awake() {
		hand = GameObject.Find("Hand");
		arrow = GameObject.Find("Arrow");
		hand.SetActive(false);
		arrow.SetActive(false);
		level = GameObject.FindObjectOfType<Level>();
		GameObject.Find("TutorialText").GetComponent<UnityEngine.UI.Text>().text = "TUTORIAL";
		level.IsTutorial = true;
		listening = true;

		DisableNotNeededGui();
		InjectTutorialAnimationTrigger();
	}

	void DisableNotNeededGui() {
		GameObject.Find("ScorePanel").SetActive(false);
		GameObject.Find("Quit").SetActive(false);
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
	}



	private int colorTimes;
	IEnumerator teachColors(){
		colorTimes = 0;
		while (colorTimes < 3){
			if (!level.Paused && listening && level.shutter.opening <= 0.1f) {
				level.Pause();
				StartCoroutine(ShowColorEquality());

			}
			yield return null;
		}
		StartCoroutine( teachTime());	
	}

	bool showedTime;
	IEnumerator teachTime(){
		colorTimes =0;
		showedTime = false;
		while(colorTimes<1){
			if(level.shutter.opening > 0.1f){
				showedTime = false;
			}
			if (!showedTime && !level.Paused && listening && level.shutter.opening <= 0.0001f ) {
				level.Pause();
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
		rend.color = Color.white;
		colorTimes++;
		level.Resume();
		showedTime = true;
	}

	IEnumerator pleaseFixIt(){
		while(true){
			listening = false;
			yield return null;
		}
	}
	
	IEnumerator ShowColorEquality() {
		//SetArrowAtCorrectColor();
		//arrow.GetComponent<SpriteRenderer>().color = HalfColor(level.qu.Color);
		//arrow.SetActive(true);
		//yield return new WaitForSeconds(1f);
		//arrow.SetActive(false);
		SetHandAtCorrectColor();
		hand.GetComponent<SpriteRenderer>().color = HalfColor(level.qu.Color);
		hand.SetActive(true);
		yield return null;
		listening = false;
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
