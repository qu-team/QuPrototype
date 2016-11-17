using UnityEngine;
using System;

namespace Gestures {

    /// <summary>
    /// Defines all gesture event types, and dispatches the events triggered by
    /// the GestureRecogniser (or by the GesturesEmulator if in debug mode).
    /// </summary>
    public class GesturesDispatcher : MonoBehaviour {

        public GesturesRecogniser gesturesRecognizer;
        public GesturesEmulator gesturesEmulator;

        public event Action<Gesture> OnGestureStart, OnGestureProgress, OnGestureEnd;
        public event Action<Tap> OnTapStart,OnTapProgress, OnTapEnd;
        public event Action<Swipe> OnSwipeStart, OnSwipeProgress, OnSwipeEnd;
        public event Action<Sprinch> OnSprinchStart, OnSprinchProgress;
        public event Action<Sprinch> OnPinchStart, OnPinchProgress, OnPinchEnd;
        public event Action<Sprinch> OnSpreadStart, OnSpreadProgress, OnSpreadEnd;

        void Awake() {
            var debug = !(Application.platform == RuntimePlatform.Android);
//			var debug = false;
            if (debug) {
                gesturesEmulator.OnClickStart += (position, duration) => NotifyGestureStart(Tap(position, duration));
                gesturesEmulator.OnClickProgress += (position, duration) => NotifyGestureProgress(Tap(position, duration));
                gesturesEmulator.OnClickEnd += (position, duration) => NotifyGestureEnd(Tap(position, duration));
                gesturesEmulator.OnDragStart += (start, end, duration) => NotifyGestureStart(Swipe(start, end, duration));
                gesturesEmulator.OnDragProgress += (start, end, duration) => NotifyGestureProgress(Swipe(start, end, duration));
                gesturesEmulator.OnDragEnd += (start, end, duration) => NotifyGestureEnd(Swipe(start, end, duration));
                gesturesEmulator.OnZoomStart += () => NotifyGestureStart(Sprinch());
                gesturesEmulator.OnZoomProgress += () => NotifyGestureProgress(Sprinch());
                gesturesEmulator.OnZoomInStart += () => NotifyGestureStart(Spread());
                gesturesEmulator.OnZoomInProgress += () => NotifyGestureProgress(Spread());
                gesturesEmulator.OnZoomInEnd += () => NotifyGestureEnd(Spread());
                gesturesEmulator.OnZoomOutStart += () => NotifyGestureStart(Pinch());
                gesturesEmulator.OnZoomOutProgress += () => NotifyGestureProgress(Pinch());
                gesturesEmulator.OnZoomOutEnd += () => NotifyGestureEnd(Pinch());
            } else {
                gesturesRecognizer.GestureStart += NotifyGestureStart;
                gesturesRecognizer.GestureProgress += NotifyGestureProgress;
                gesturesRecognizer.GestureEnd += NotifyGestureEnd;
            }
        }

        void NotifyGestureStart(Gesture gesture) {
            Trigger(OnGestureStart, gesture);
            switch (gesture.Type) {
                case GestureType.TAP: Trigger(OnTapStart, gesture as Tap); break;
                case GestureType.SWIPE: Trigger(OnSwipeStart, gesture as Swipe); break;
                case GestureType.SPRINCH: Trigger(OnSprinchStart, gesture as Sprinch); break;
                case GestureType.PINCH: Trigger(OnPinchStart, gesture as Sprinch); break;
                case GestureType.SPREAD: Trigger(OnSpreadStart, gesture as Sprinch); break;
            }
        }

        void NotifyGestureProgress(Gesture gesture) {
            Trigger(OnGestureProgress, gesture);
            switch (gesture.Type) {
                case GestureType.TAP: Trigger(OnTapProgress, gesture as Tap); break;
                case GestureType.SWIPE: Trigger(OnSwipeProgress, gesture as Swipe); break;
                case GestureType.SPRINCH: Trigger(OnSprinchProgress, gesture as Sprinch); break;
                case GestureType.PINCH: Trigger(OnPinchProgress, gesture as Sprinch); break;
                case GestureType.SPREAD: Trigger(OnSpreadProgress, gesture as Sprinch); break;
            }
        }

        void NotifyGestureEnd(Gesture gesture) {
            Trigger(OnGestureEnd, gesture);
            switch (gesture.Type) {
                case GestureType.TAP: Trigger(OnTapEnd, gesture as Tap); break;
                case GestureType.SWIPE: Trigger(OnSwipeEnd, gesture as Swipe); break;
                case GestureType.PINCH: Trigger(OnPinchEnd, gesture as Sprinch); break;
                case GestureType.SPREAD: Trigger(OnSpreadEnd, gesture as Sprinch); break;
            }
        }

        static void Trigger<T>(Action<T> handler, T value) {
            if (handler != null) { handler.Invoke(value); }
        }

        Tap Tap(Vector2 position, float duration) {
            var tap = new Tap(position, Time.time - duration);
            tap.EndTime = Time.time;
            return tap;
        }

        Swipe Swipe(Vector2 start, Vector2 end, float time) {
            var swipe = new Swipe(start, Time.time - time);
            swipe.End = end;
            swipe.EndTime = Time.time;
            return swipe;
        }

        Sprinch Sprinch() {
            var sprinch = new Sprinch(Vector2.zero, Vector2.zero);
            sprinch.EndPoints = new Vector2[] { Vector2.zero, Vector2.zero };
            return sprinch;
        }

        Sprinch Pinch() {
            var pinch = new Sprinch(Vector2.zero, new Vector2(100f, 100f));
            pinch.EndPoints = new Vector2[] { Vector2.zero, Vector2.zero };
            return pinch;
        }

        Sprinch Spread() {
            var spread = new Sprinch(Vector2.zero, Vector2.zero);
            spread.EndPoints = new Vector2[] { Vector2.zero, new Vector2(100f, 100f) };
            return spread;
        }
    }
}
