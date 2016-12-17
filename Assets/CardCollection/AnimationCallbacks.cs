using UnityEngine;

public class AnimationCallbacks:MonoBehaviour{

	public CardCollectionManager cardManager;

	public void AnimationEnded(){
		cardManager.AnimationEnded();
	}
	public void ChangeDelayedCard(){
		cardManager.DisplayDelayedCard();
	}
}
