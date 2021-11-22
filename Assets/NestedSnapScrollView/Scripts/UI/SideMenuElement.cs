using System;
using System.Collections;
using NestedScroll.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NestedScroll.ScrollElement
{
    public class SideMenuElement : MonoBehaviour
    {
        [SerializeField] private MenuOrientation _menuOrientation;
        [SerializeField] private MenuShowHideAnimation _menuShowAnimation;
        [SerializeField] private MenuShowHideAnimation _menuHideAnimation;
        [Space] [SerializeField] RectTransform _menuElement;
        [SerializeField] RectTransform _contentRectTransform;
        [SerializeField] NestedScrollView _nestedScrollView;
        [SerializeField] HorizontalLayoutGroup _horizontalLayout;
        [Space] [SerializeField] float _swipeSensitivity;
        [SerializeField] private float _snapSpeed;
        [SerializeField] private float _scaleAnimationSpeed;

        private bool _isMenuVisible;
        private bool _isOnTransitionNow;
        private bool _isInitialized;

        private Vector2 _startMenuSize;

        public void OnDrag(PointerEventData eventData)
        {
            CheckSwipe(eventData);
        }

        private void CheckSwipe(PointerEventData eventData)
        {
            if (!_isInitialized || _isOnTransitionNow)
                return;

            bool isShowMenuGesture = SwipeOrientation(eventData) == _menuOrientation;

            if (isShowMenuGesture)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }

        private void ShowMenu()
        {
            if (_menuElement == null || _isMenuVisible)
                return;

            _menuElement.gameObject.SetActive(true);
            _isMenuVisible = true;

            if (_menuShowAnimation == MenuShowHideAnimation.None)
            {
                if (_menuHideAnimation == MenuShowHideAnimation.WidthShift ||
                    _menuHideAnimation == MenuShowHideAnimation.WidthShiftAndMove)
                {
                    SetDefaultMenuScale();
                }

                StartCoroutine(MoveContent(true));
            }
            else if (_menuShowAnimation == MenuShowHideAnimation.WidthShift)
            {
                StartCoroutine(ScaleMenuElement(false));
            }
            else if (_menuShowAnimation == MenuShowHideAnimation.WidthShiftAndMove)
            {
                StartCoroutine(ScaleMenuElement(false, () => { StartCoroutine(MoveContent(true)); }));
            }
        }

        private void HideMenu()
        {
            if (_menuElement == null || !_isMenuVisible)
                return;

            _isMenuVisible = false;

            if (_menuHideAnimation == MenuShowHideAnimation.None)
            {
                StartCoroutine(MoveContent(false, () => { _menuElement.gameObject.SetActive(false); }));
            }
            else if (_menuHideAnimation == MenuShowHideAnimation.WidthShift ||
                     _menuHideAnimation == MenuShowHideAnimation.WidthShiftAndMove)
                StartCoroutine(ScaleMenuElement(true, () => { _menuElement.gameObject.SetActive(false); }));
        }

        private void SetDefaultMenuScale()
        {
            _menuElement.sizeDelta = _startMenuSize;
        }

        private IEnumerator ScaleMenuElement(bool isHideAnimation, Action OnFinishCallback = null)
        {
            _nestedScrollView.horizontal = false;

            _isOnTransitionNow = true;

            Vector2 xSize = isHideAnimation ? _startMenuSize : new Vector2(0, _startMenuSize.y);

            while (isHideAnimation ? (xSize.x > 0) : (xSize.x < _startMenuSize.x))
            {
                _menuElement.sizeDelta = xSize;

                if (isHideAnimation)
                {
                    xSize.x -= _scaleAnimationSpeed;
                }
                else
                {
                    xSize.x += _scaleAnimationSpeed;
                }

                yield return null;
            }

            _isOnTransitionNow = false;

            _nestedScrollView.horizontal = true;
            _nestedScrollView.velocity = Vector2.zero;

            OnFinishCallback?.Invoke();
        }

        private IEnumerator MoveContent(bool isShow, Action onMoveFinishedCallback = null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRectTransform);

            _nestedScrollView.horizontal = false;

            _isOnTransitionNow = true;

            float targetPosition = 0;

            if (_menuOrientation == MenuOrientation.Right)
            {
                targetPosition = isShow ? -(_contentRectTransform.sizeDelta.x) : 0;
            }
            else
            {
                if (!isShow)
                {
                    targetPosition = -_contentRectTransform.sizeDelta.x;
                }
                else
                {
                    _contentRectTransform.anchoredPosition = -_contentRectTransform.sizeDelta;
                }
            }

            while (!IsContentMovedToPosition(_contentRectTransform.anchoredPosition.x, targetPosition))
            {
                Vector2 moveOffset = new Vector2(Mathf.SmoothStep(_contentRectTransform.anchoredPosition.x,
                    targetPosition,
                    _snapSpeed * Time.fixedDeltaTime), 0);

                _contentRectTransform.anchoredPosition = moveOffset;
                yield return null;
            }

            _isOnTransitionNow = false;

            _contentRectTransform.anchoredPosition = new Vector2(targetPosition, 0);

            _nestedScrollView.horizontal = true;
            _nestedScrollView.velocity = Vector2.zero;

            onMoveFinishedCallback?.Invoke();
        }

        private bool IsContentMovedToPosition(float currentPositionX, float targetPositionX)
        {
            float finishTreshold = 5f;
            bool isFinishReached = (currentPositionX >= targetPositionX - finishTreshold) &&
                                   (currentPositionX <= targetPositionX + finishTreshold);

            return isFinishReached;
        }

        private MenuOrientation SwipeOrientation(PointerEventData eventData)
        {
            MenuOrientation swipeOrientation = MenuOrientation.None;

            if (Math.Abs(eventData.delta.x) >= _swipeSensitivity)
            {
                swipeOrientation = eventData.delta.x > 0 ? MenuOrientation.Left : MenuOrientation.Right;
            }

            return swipeOrientation;
        }

        #region Subscriptions

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }

        private void Initialize()
        {
            _startMenuSize = _menuElement.sizeDelta;
            SubscribeEvents();

            _isInitialized = true;
        }

        private void SubscribeEvents()
        {
            _nestedScrollView.OnHorizontalDraggingNow += OnDrag;
        }

        private void UnSubscribeEvents()
        {
            _nestedScrollView.OnHorizontalDraggingNow -= OnDrag;
        }

        #endregion
    }

    public enum MenuOrientation
    {
        None = 0,
        Right = 1,
        Left = 2,
    }

    public enum MenuShowHideAnimation
    {
        None = 0,
        WidthShift = 1,
        WidthShiftAndMove = 2,
    }
}
