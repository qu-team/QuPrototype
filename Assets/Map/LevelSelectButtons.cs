using UnityEngine;

public class LevelSelectButtons : MonoBehaviour{
	public static Sprite unlockedSprite;
	public static Sprite locked;
	public static Sprite completed;
	public GameObject[] stars;

	public int level;
	public bool unlocked;

	void Start(){
		var renderer = GetComponent<SpriteRenderer>();
		if(unlockedSprite == null){
			unlockedSprite = Resources.Load<Sprite>("qU");
			locked = Resources.Load<Sprite>("qULocked");
			completed = Resources.Load<Sprite>("quCompleted");
		}
		if( (int)GameData.data.curLevelUnlocked < level ) {
			renderer.sprite = locked;
			//Debug.Log( "Setting level "+level+" to "+GameData.data.curLevelUnlocked);
		}
		else{
			if( UnlockConditions.NStars(level) == 3){
				renderer.sprite = completed;
			}else{
				renderer.sprite = unlockedSprite;;
			}
		}
		for(int i =0;i<3;i++){
			if(i> UnlockConditions.NStars( level )-1){
				stars[i].GetComponent<SpriteRenderer>().color = Color.grey;	
			}else{
				stars[i].GetComponent<SpriteRenderer>().color = Color.yellow;
			}
		}
	}
	
    public Color Color {
        get {
            var sprite = GetComponent<SpriteRenderer>();
            return sprite != null ? sprite.color : Color.white;
        }
    }
}
