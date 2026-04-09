using UnityEngine;
using UnityEngine.EventSystems;

namespace GimGim.AudioManagement {
    public class AudioClipTrimmerView : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler {
        [SerializeField] private AudioClipController controller;
        [SerializeField] private RectTransform waveform;
        [SerializeField] private RectTransform trimStartHandle;
        [SerializeField] private RectTransform trimEndHandle;
        [SerializeField] private float handleGrabWidth = 40f;
        
        private DragTarget _dragTarget = DragTarget.None;
        private const float DRAG_CLAMP_THRESHOLD = 0.03f;

        private void Awake() {
            SetHandleX(trimStartHandle, 0f);
            SetHandleX(trimEndHandle, 1f);
        }

        private void OnEnable() {
            controller.ClipDataChanged += OnClipDataChanged;
            controller.TrimChanged += OnTrimChanged;
        }

        private void OnDisable() {
            controller.ClipDataChanged -= OnClipDataChanged;
            controller.TrimChanged -= OnTrimChanged;
        }

        /// <summary>
        /// Handles a click on the waveform rect. Converts the pointer position to a
        /// normalized time and starts playback from that point.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData) {
            if (_dragTarget != DragTarget.None)
                return;

            if (controller.ClipData == null)
                return;

            AudioClipData clipData = controller.ClipData;
            float normalizedX = GetNormalizedX(eventData.position);

            if (normalizedX < clipData.trimStart || normalizedX > clipData.trimEnd)
                return;
            
            controller.PlayClip(clipData.trimStart, clipData.trimEnd);
        }
        
        /// <summary>
        /// Determines which handle (if any) the drag started on.
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData) {
            if (controller.ClipData == null)
                return;
            
            float normalizedX = GetNormalizedX(eventData.position);
            
            AudioClipData clipData = controller.ClipData;
            float startNormalizedX = clipData.trimStart;
            float endNormalizedX = clipData.trimEnd;

            float distStart = Mathf.Abs(normalizedX - startNormalizedX);
            float distEnd = Mathf.Abs(normalizedX - endNormalizedX);

            float grabThreshold = handleGrabWidth / waveform.rect.width;

            if (distStart < grabThreshold && distStart <= distEnd) {
                _dragTarget = DragTarget.Start;
            }
            else if (distEnd < grabThreshold) {
                _dragTarget = DragTarget.End;
            }
            else {
                _dragTarget = DragTarget.None;
            }
        }
        
        /// <summary>
        /// Updates the active trim handle position while dragging.
        /// </summary>
        public void OnDrag(PointerEventData eventData) {
            if (_dragTarget == DragTarget.None)
                return;

            if (controller.ClipData == null)
                return;
            
            float normalizedX = GetNormalizedX(eventData.position);
            
            AudioClipData clipData = controller.ClipData;
            float start = clipData.trimStart;
            float end = clipData.trimEnd;

            if (_dragTarget == DragTarget.Start) {
                start = Mathf.Clamp(normalizedX, 0f, end - DRAG_CLAMP_THRESHOLD);
            }
            else if (_dragTarget == DragTarget.End) {
                end = Mathf.Clamp(normalizedX, start + DRAG_CLAMP_THRESHOLD, 1f);
            }

            controller.SetTrim(start, end);
        }
        
        /// <summary>
        /// Resets the drag target to none.
        /// </summary>
        public void OnEndDrag(PointerEventData eventData) {
            _dragTarget = DragTarget.None;
        }

        private void OnClipDataChanged(AudioClipData clipData) {
            UpdateHandlePositions(clipData);
        }

        private void OnTrimChanged(AudioClipData clipData) {
            UpdateHandlePositions(clipData);
        }

        /// <summary>
        /// Repositions the trim handle RectTransforms to match the current trim values.
        /// Handles are positioned using anchoredPosition along the waveform width.
        /// </summary>
        private void UpdateHandlePositions(AudioClipData clipData) {
            SetHandleX(trimStartHandle, clipData.trimStart);
            SetHandleX(trimEndHandle, clipData.trimEnd);
        }
        
        /// <summary>
        /// Repositions a trim handle to a normalized x position within the waveform rect.
        /// </summary>
        /// <param name="handle">The handle RectTransform to reposition.</param>
        /// <param name="normalizedX">Normalized position (0-1) along the waveform width.</param>
        private void SetHandleX(RectTransform handle, float normalizedX) {
            Rect rect = waveform.rect;
            
            float waveformWidth = rect.width;
            float anchorOffsetX = handle.anchorMin.x * waveformWidth;

            float localX = Mathf.Lerp(rect.xMin, rect.xMax, normalizedX) - anchorOffsetX;

            Vector2 anchoredPos = handle.anchoredPosition;
            anchoredPos.x = localX + rect.width / 2;
            handle.anchoredPosition = anchoredPos;
        }

        /// <summary>
        /// Converts a screen-space pointer position to a normalized (0–1) x position within <see cref="waveform"/>.
        /// </summary>
        private float GetNormalizedX(Vector2 screenPos) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(waveform, screenPos,
                null, out Vector2 localPoint);
            
            Rect rect = waveform.rect;

            float normalizedX = (localPoint.x - rect.xMin) / rect.width;
            
            return Mathf.Clamp01(normalizedX);
        }

        private enum DragTarget {
            None,
            Start,
            End
        }
    }
}