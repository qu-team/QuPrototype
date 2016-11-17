using UnityEngine;

namespace Gestures {

    //TODO figlio dello swipe?
    public class Sprinch : Gesture {

        const float PERCENTAGE_TO_CANCEL = 0.15f;
        const float TOLERANCE = 15f;

        float initialDistance;
        bool canceled;
        Vector2[] start = new Vector2[2];
        Vector2[] endPoints = new Vector2[2];
        float maxminDist; //FIXME servirà?
        float percentage = 0f;

        public Sprinch(Vector2 start1, Vector2 start2) : base(GestureType.SPRINCH) {
            start[0] = start1;
            start[1] = start2;
            initialDistance = Vector2.Distance(start1, start2);
        }

        /// <summary>Initial points of the gesture.</summary>
        public Vector2[] Start { get { return start; } }

        public bool Canceled { get { return canceled; } }

        public float Percentage { get { return percentage; } }

	public float CurDist { get { return Vector2.Distance(EndPoints[0], EndPoints[1]); } }	 

        public Vector2[] EndPoints {
            get { return endPoints; }
            set {
                endPoints = value;
                float currDist = Vector2.Distance(endPoints[0], endPoints[1]);
                switch (type) {
                    case GestureType.SPRINCH:
                        if (Mathf.Abs(currDist - initialDistance) > TOLERANCE) {
                            type = (currDist < initialDistance) ? GestureType.PINCH : GestureType.SPREAD;
                        }
                        UpdateOnPinch(currDist);
                        UpdateOnSpread(currDist);
                        break;
                    case GestureType.PINCH:
                        UpdateOnPinch(currDist); break;
                    case GestureType.SPREAD:
                        UpdateOnSpread(currDist); break;
                }
                canceled = percentage <= PERCENTAGE_TO_CANCEL;
            }

        }

        void UpdateOnPinch(float currDist) {
            maxminDist = currDist < maxminDist ? currDist : maxminDist;
            percentage = 1f - currDist / initialDistance;
            percentage = percentage < 0f ? 0f : percentage;
        }

        void UpdateOnSpread(float currDist) {
            maxminDist = currDist > maxminDist ? currDist : maxminDist;
            percentage = (currDist - initialDistance) / (maxminDist - initialDistance);
            percentage = percentage > 0f ? percentage : 0f;
        }
    }
}
