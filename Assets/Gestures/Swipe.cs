using UnityEngine;
using System.Collections;

namespace Gestures {

    public class Swipe : Gesture {

        public const float PERCENT_TO_CANCEL = 0.10f;

        float maxLen;
        bool canceled;
        float length;
        Vector2 start;
        Vector2 end;
        float startTime;

        public Swipe(Vector2 start, float startTime) : base(GestureType.SWIPE) {
            this.start = start;
            this.startTime = startTime;
            EndTime = startTime;
        }

        /// <summary>Gets if the swipe counts as canceled.</summary>
        /// <value><c>true</c> if this swipe has been canceled; otherwise, <c>false</c>.</value>
        public bool Canceled { get { return canceled; } }

        /// <summary>Gets the lenght in px.</summary>
        public float Lenght { get { return length; } }

        public Vector2 Start { get { return start; } }

        public Vector2 End {
            get { return end; }
            set {
                length = Vector2.Distance(start, value);
                maxLen = length > maxLen ? length : maxLen;
                canceled = length / maxLen < PERCENT_TO_CANCEL;
                end = value;
            }
        }

        public float StartTime { get { return startTime; } }

        public float EndTime { get; set; }

        public float Duration { get { return EndTime - startTime; } }
    }
}
