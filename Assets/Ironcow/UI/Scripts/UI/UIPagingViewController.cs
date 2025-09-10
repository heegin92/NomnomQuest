// ─────────────────────────────────────────────────────────────────────────────
// Part of the Synapse Framework © 2025 Ironcow Studio
// Distributed via Gumroad under a paid license
// 
// 🔐 This file is part of a licensed product. Redistribution or sharing is prohibited.
// 🔑 A valid license key is required to unlock all features.
// 
// 🌐 For license terms, support, or team licensing, visit:
//     https://ironcowstudio.duckdns.org/ironcowstudio.html
// ─────────────────────────────────────────────────────────────────────────────


using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ironcow.Synapse.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(ScrollRect))]
    public class UIPagingViewController : SynapseBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [Header("Layout & Animation")]
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private float animationDuration = 0.3f;

        [Header("References")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject root;

        [Header("Animation Curve Tangents")]
        [SerializeField] private List<float> inTangent = new List<float>() { 0f, 1f };
        [SerializeField] private List<float> outTangent = new List<float>() { 1f, 0f };

        [Header("Events")]
        public UnityAction<int> OnPageChanged;
        public UnityAction OnMoveStart;
        public UnityAction OnMoveEnd;

        private RectTransform rectTransform;
        private Rect currentViewRect;

        private AnimationCurve curve;
        private Vector2 destPosition;
        private Vector2 initialPosition;
        private float animationStartTime;

        private bool isAnimating = false;

        private int currentPageIndex = 0;
        private int targetPageIndex = 0;

        public void OnBeginDrag(PointerEventData eventData)
        {
            isAnimating = false;
            OnMoveStart?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            CalculatePageFromDrag(eventData.delta);
        }

        private void CalculatePageFromDrag(Vector2 delta, int overrideTarget = -1)
        {
            scrollRect.StopMovement();

            float pageSize = scrollRect.horizontal
                ? -(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x)
                : -(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y);

            int newIndex = Mathf.RoundToInt(scrollRect.content.anchoredPosition.x / pageSize);
            if (overrideTarget >= 0)
                newIndex = overrideTarget;

            // 강제 스와이프 판정
            if (newIndex == currentPageIndex)
            {
                if (scrollRect.horizontal && Mathf.Abs(delta.x) >= 4)
                    newIndex += (int)Mathf.Sign(-delta.x);

                if (scrollRect.vertical && Mathf.Abs(delta.y) >= 4)
                    newIndex += (int)Mathf.Sign(-delta.y);
            }

            MoveToPage(newIndex);
        }

        private void MoveToPage(int index)
        {
            int maxPage = gridLayoutGroup.transform.childCount - 1;
            targetPageIndex = Mathf.Clamp(index, 0, maxPage);

            if (targetPageIndex != currentPageIndex)
                OnPageChanged?.Invoke(targetPageIndex);

            Vector2 targetPos = scrollRect.horizontal
                ? new Vector2(targetPageIndex * -(gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x), scrollRect.content.anchoredPosition.y)
                : new Vector2(scrollRect.content.anchoredPosition.x, targetPageIndex * -(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));

            StartAnimation(scrollRect.content.anchoredPosition, targetPos);
            currentPageIndex = targetPageIndex;

            OnMoveEnd?.Invoke();
        }

        private void StartAnimation(Vector2 from, Vector2 to)
        {
            animationStartTime = Time.time;
            initialPosition = from;
            destPosition = to;

            curve = new AnimationCurve(
                new Keyframe(0f, 0f, inTangent[0], outTangent[0]),
                new Keyframe(animationDuration, 1f, inTangent[1], outTangent[1])
            );

            isAnimating = true;
        }

        private void LateUpdate()
        {
            if (!isAnimating) return;

            float elapsed = Time.time - animationStartTime;
            if (elapsed >= animationDuration)
            {
                scrollRect.content.anchoredPosition = destPosition;
                isAnimating = false;
                return;
            }

            float t = curve.Evaluate(elapsed);
            scrollRect.content.anchoredPosition = Vector2.Lerp(initialPosition, destPosition, t);
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();
        }

        private void Start()
        {
            UpdatePadding();
        }

        private void Update()
        {
            if (rectTransform.rect.size != currentViewRect.size)
                UpdatePadding();
        }

        private void UpdatePadding()
        {
            currentViewRect = rectTransform.rect;

            int horizontalPadding = Mathf.RoundToInt((currentViewRect.width - gridLayoutGroup.cellSize.x) / 2.0f);
            int verticalPadding = Mathf.RoundToInt((currentViewRect.height - gridLayoutGroup.cellSize.y) / 2.0f);
            gridLayoutGroup.padding = new RectOffset(horizontalPadding, horizontalPadding, verticalPadding, verticalPadding);
        }
    }
}
