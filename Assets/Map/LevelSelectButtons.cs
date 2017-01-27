using UnityEngine;

public class LevelSelectButtons : MonoBehaviour{
	public int level;
	public bool unlocked;

    public Color Color {
        get {
            var sprite = GetComponent<SpriteRenderer>();
            return sprite != null ? sprite.color : Color.white;
        }
    }
}
