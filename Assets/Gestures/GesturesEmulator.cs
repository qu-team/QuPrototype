using UnityEngine;
using System;

namespace Gestures {

	using Input = UnityEngine.Input;
    public class GesturesEmulator : MonoBehaviour {

        public float MinDragSpan;

        const int LEFT_BUTTON = 0;

        bool dragging;
        Vector2 dragStartPosition;
        float dragStartTime;
        ZoomState zooming = ZoomState.NONE;

        public event Action<Vector2, float> OnClickStart,OnClickProgress, OnClickEnd;
        public event Action<Vector2, Vector2, float> OnDragStart, OnDragProgress, OnDragEnd;
        public event Action OnZoomStart, OnZoomProgress;
        public event Action OnZoomInStart, OnZoomInProgress, OnZoomInEnd;
        public event Action OnZoomOutStart, OnZoomOutProgress, OnZoomOutEnd;

        void Update() {
            HandleDrag();
            HandleZoom();
        }

        void HandleDrag() {
            if (Input.GetMouseButton(LEFT_BUTTON)){
				if (Input.GetMouseButtonDown(LEFT_BUTTON)) {
					dragging = false;
					dragStartPosition = Input.mousePosition;
					dragStartTime = Time.time;
					if (OnClickStart != null) { OnClickStart(dragStartPosition, 0); }
				} else {
					if (OnClickProgress!=null) {
						OnClickProgress(dragStartPosition,Time.time - dragStartTime);
					}
				}
			}
			
            Vector2 dragEndPosition = Input.mousePosition;
            var duration = Time.time - dragStartTime;
            if (!dragging && Input.GetMouseButton(LEFT_BUTTON)) {
                var span = (dragEndPosition - dragStartPosition).magnitude;
                if (span >= MinDragSpan) {
                    dragging = true;
                    if (OnDragStart != null) { OnDragStart(dragStartPosition, dragEndPosition, duration); }
                }
            }
            if (Input.GetMouseButtonUp(LEFT_BUTTON)) {
                if (dragging) {
                    if (OnDragEnd != null) { OnDragEnd(dragStartPosition, dragEndPosition, duration); }
                    dragging = false;
                } else {
                    if (OnClickEnd != null) { OnClickEnd(dragStartPosition, duration); }
                }
            } else if (dragging) {
                if (OnDragProgress != null) { OnDragProgress(dragStartPosition, dragEndPosition, duration); }
            }
        }

        void HandleZoom() {
            if (Zooming && Input.GetKeyUp(KeyCode.LeftShift)) {
                if (GetZoomInKey) { Trigger(OnZoomInEnd); }
                if (GetZoomOutKey) { Trigger(OnZoomOutEnd); }
                zooming = ZoomState.NONE;
                return;
            }
            if (ZoomingIn && Input.GetKey(KeyCode.LeftShift) && GetZoomInKey) {
                Trigger(OnZoomInProgress);
                return;
            }
            if (ZoomingOut && Input.GetKey(KeyCode.LeftShift) && GetZoomOutKey) {
                Trigger(OnZoomOutProgress);
                return;
            }
            if (UndefinedZooming && Input.GetKey(KeyCode.LeftShift) && !GetZoomInKey && !GetZoomOutKey) {
                Trigger(OnZoomProgress);
                return;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                Trigger(OnZoomStart);
                zooming = ZoomState.ZOOMING;
                return;
            }
            if (!ZoomingIn && GetZoomInKey) {
                Trigger(OnZoomInStart);
                zooming = ZoomState.ZOOMING_IN;
                return;
            }
            if (!ZoomingOut && GetZoomOutKey) {
                Trigger(OnZoomOutStart);
                zooming = ZoomState.ZOOMING_OUT;
                return;
            }
        }

        void Trigger(Action action) {
            if (action != null) { action.Invoke(); }
        }

        bool UndefinedZooming { get { return zooming == ZoomState.ZOOMING; } }
        bool ZoomingIn { get { return zooming == ZoomState.ZOOMING_IN; } }
        bool ZoomingOut { get { return zooming == ZoomState.ZOOMING_OUT; } }
        bool Zooming { get { return UndefinedZooming || ZoomingIn || ZoomingOut; } }
        bool GetZoomInKey { get { return Input.GetKey(KeyCode.W); } }
        bool GetZoomOutKey { get { return Input.GetKey(KeyCode.S); } }

        enum ZoomState {
            NONE, ZOOMING, ZOOMING_IN, ZOOMING_OUT
        }
    }
}
