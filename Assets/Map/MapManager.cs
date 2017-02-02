using UnityEngine;
using Gestures;

public class MapManager : MonoBehaviour{
    public GesturesDispatcher dispatcher;
    public LevelPopup popup;
    Vector3 orig;
    bool justClosed;
	public Vector2 lowerBounds;
	public Vector2 upperBounds;


    void Start(){
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
		orig = Camera.main.transform.position;
        GameManager.Instance.MapFinishedLoading(this);
        dispatcher.OnTapEnd+= TapEnd;
        dispatcher.OnSwipeProgress += SwipeProgress;
        dispatcher.OnSwipeStart += SwipeStart;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
    }

    public void ClickedLevel(LevelSelectButtons level){
        popup.ShowPopup(GameManager.Instance, level.level, level.Color);
    }

    void TapEnd(Tap tap){
        if(justClosed){
            justClosed = false;
            return;
        }
        if(popup.gameObject.activeSelf ) return;
        RaycastHit hit = new RaycastHit();
		Vector3 s2w = Camera.main.ScreenToWorldPoint(
				new Vector3(tap.Position.x,
					tap.Position.y,
					Camera.main.nearClipPlane));
		if(!Physics.Raycast(Camera.main.transform.position, s2w - Camera.main.transform.position, out hit))
			return;
        LevelSelectButtons lv = hit.collider.gameObject.GetComponent<LevelSelectButtons>();
        if (lv != null){
            ClickedLevel(lv);
        }
    }

    void SwipeProgress(Swipe swipe) {    
        if(popup.gameObject.activeSelf ) return;
        Camera camera = Camera.main;
        Vector3 shift = -(camera.ScreenToWorldPoint(
					new Vector3(swipe.End.x, swipe.End.y, orig.z)) -
                camera.ScreenToWorldPoint(new Vector3(swipe.Start.x,swipe.Start.y, orig.z)));
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

    void MoveCamera(Vector3 shift){
        Camera camera = Camera.main;
        var level = GameObject.Find("Level" + (GameData.data.curLevelUnlocked + 1));
	var upperY = Mathf.Min(upperBounds.y, level.transform.position.y + 2);
        camera.transform.position = new Vector3(
                Mathf.Clamp(orig.x - shift.x, lowerBounds.x, upperBounds.x),
                Mathf.Clamp(orig.y - shift.y, lowerBounds.y, upperY), orig.z);

    }

    public void MoveCameraAtLevel(int lvl){
        var level = GameObject.Find("Level" + (lvl + 1));
        if (level == null) { return; }
        MoveCamera(-level.transform.position + Camera.main.transform.position);
    }

    public void BackButton() {
        GameManager.Instance.Back();
    }
}
