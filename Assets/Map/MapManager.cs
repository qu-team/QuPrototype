using UnityEngine;
using Gestures;

public class MapManager : MonoBehaviour{
	GameManager gm;
	public GesturesDispatcher dispatcher;
	public LevelPopup popup;
	public Vector2 mapDimension;
	Vector3 orig;
	bool justClosed;


	void Start(){
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
		SpriteRenderer sp = GetComponent<SpriteRenderer>();
		mapDimension = new Vector2(sp.bounds.extents.x ,sp.bounds.extents.y);
		gm.MapFinishedLoading(this);
		dispatcher.OnTapEnd+= TapEnd;
		dispatcher.OnSwipeProgress += SwipeProgress;
		dispatcher.OnSwipeStart += SwipeStart;	
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Q))
			Application.Quit();
	}

	public void ClickedLevel(int level){
		popup.ShowPopup(gm, level);	
	}

	void TapEnd(Tap tap){
		if(justClosed){
			justClosed = false;
			return;
		}
		if(popup.gameObject.activeSelf ) return;
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(tap.Position), Vector2.zero);
		if(hit.collider==null) return;
		LevelSelectButtons lv = hit.collider.gameObject.GetComponent<LevelSelectButtons>();
		if( lv != null){
			ClickedLevel(lv.level);
		}
	}

	void SwipeProgress(Swipe swipe) {	
		if(popup.gameObject.activeSelf ) return;
		Camera camera = Camera.main;
		Vector2 shift = (camera.ScreenToWorldPoint(swipe.End) -
				camera.ScreenToWorldPoint(swipe.Start));
		MoveCamera(shift);		
	}
	void SwipeStart(Swipe swipe) {
		if(popup.gameObject.activeSelf) return;
		orig =Camera.main.transform.position;
	}
	public void ClosePopup(){
		justClosed= true;
		popup.gameObject.SetActive(false);
	}

	void MoveCamera(Vector2 shift){
		Camera camera = Camera.main;
		float xbound = mapDimension.x - camera.orthographicSize * camera.aspect,
		      ybound = mapDimension.y - camera.orthographicSize;
		camera.transform.position = new Vector3(
				Mathf.Clamp(orig.x - shift.x, -xbound, xbound),
				Mathf.Clamp(orig.y - shift.y, -ybound, ybound), orig.z);

	}

	public void MoveCameraAtLevel(int lvl){
		Vector2 target = GameObject.Find("Level" + (lvl+1)).transform.position;
		MoveCamera(-target + (Vector2)Camera.main.transform.position);
	}
}
