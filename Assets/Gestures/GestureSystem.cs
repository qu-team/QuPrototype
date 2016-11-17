using UnityEngine;

namespace Gestures {

public static class GestureSystem {
	private static GameObject gestureSystem;

	public static GameObject instance {
		get {
			if (gestureSystem == null)
				gestureSystem = GameObject.Find("GestureSystem");
			return gestureSystem;
		}
		private set { gestureSystem = value; }
	}

	public static GesturesDispatcher dispatcher {
		get { return instance.GetComponent<GesturesDispatcher>(); }
		private set {}
	}
}

}
