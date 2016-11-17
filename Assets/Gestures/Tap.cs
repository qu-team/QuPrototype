using UnityEngine;
using System.Collections;

namespace Gestures {

    public class Tap : Gesture {

        Vector2 position;
        float startTime;

        public Tap(Vector2 position, float startTime) : base(GestureType.TAP) {
            this.position = position;
            this.startTime = startTime;
            EndTime = startTime;
        }

        public Vector2 Position { get { return position; } }

        public float StartTime { get { return startTime; } }

        public float EndTime { get; set; }

        public float Duration { get { return EndTime - startTime; } }
    }
}
