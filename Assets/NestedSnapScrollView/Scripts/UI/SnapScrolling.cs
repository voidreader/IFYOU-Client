using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NestedScroll.ScrollElement
{
    public class SnapScrolling : MonoBehaviour
    {
        [Header("Controllers")] [HideInInspector]
        public int PanCount;

        [Range(0f, 20f)] public float SnapSpeed;
        [Range(0f, 1.5f)] public float SnapPointOffset;
        
        [Tooltip("Swipe sensitivity")][Range(100f, 1000f)] public float InertiaDisablingScrollVelocity;
        public bool InverseScrollRect;

        [Header("Snap ignore elements count (from back)")]
        public int LastIgnoredElementsCount;

        public ScrollRect scrollRect;
        [Header("Showing border point")] public bool IsShowHelpers;

        private int _startLastIgnoredElementsCount;

        private GameObject[] _instPans;
        private Vector2[] _pansPosLocal;
        private Vector2[] _snappingElementsSize;
        private Vector2[] _elementPosWithOffset;

        private RectTransform _contentRect;
        private Vector2 _contentVector;

        private int _selectedPanID;
        private bool _isScrolling;

        private bool _isInitialized;

        private void Awake()
        {
            _contentRect = GetComponent<RectTransform>();
            _startLastIgnoredElementsCount = LastIgnoredElementsCount;
        }

        //***IMPORTANT***
        /// <summary>
        /// When you add or remove elements from scroll content - call this method
        /// </summary>
        public void UpdateScroll()
        {
            StartCoroutine(UpdateSnap());
        }

        /// <summary>
        /// When scroll rect dragging
        /// </summary>
        /// <param name="scroll">Is dragging now</param>
        public void Scrolling(bool scroll)
        {
            _isScrolling = scroll;
            if (scroll)
                scrollRect.inertia = true;
        }

        public void UpdateElementsInfo()
        {
            PanCount = transform.childCount;

            if (LastIgnoredElementsCount >= PanCount)
                LastIgnoredElementsCount = 0;
            else
                LastIgnoredElementsCount = _startLastIgnoredElementsCount;

            _pansPosLocal = new Vector2[PanCount];
            _snappingElementsSize = new Vector2[PanCount];
            for (int i = 0; i < PanCount; i++)
            {
                _snappingElementsSize[i] = transform.GetChild(i).GetComponent<RectTransform>().sizeDelta;
                _pansPosLocal[i] = InverseScrollRect
                    ? -transform.GetChild(i).gameObject.transform.localPosition
                    : transform.GetChild(i).gameObject.transform.localPosition;
            }
        }

        private void Start()
        {
            UpdateScroll();
        }

        /// <summary>
        /// Call this method when will added new elements on content
        /// Updating element count and recreate element snap points
        /// </summary>   
        private IEnumerator UpdateSnap()
        {
            yield return new WaitForEndOfFrame();
            UpdateElementsInfo();
            UpdateOffsetedElementList();
            CalculateLastIndexElement();

            _isInitialized = true;
        }

        /// <summary>
        /// Recalculate elements snap points with offset parameter
        /// </summary>
        private void UpdateOffsetedElementList()
        {
            _elementPosWithOffset = new Vector2[PanCount];
            for (int i = 0; i < PanCount; i++)
            {
                Vector3 gloabalPos = _pansPosLocal[i];
                Vector3 gloabalScale = transform.GetChild(i).GetComponent<RectTransform>().sizeDelta;
                _elementPosWithOffset[i] =
                    new Vector2(gloabalPos.x + (gloabalScale.x / 2 * SnapPointOffset), gloabalPos.y);

            }
        }

        private void Update()
        {
            if (_isInitialized && _elementPosWithOffset.Length > 0)
            {
                Snap();
            }
        }

        private void Snap()
        {
            if (_contentRect.anchoredPosition.x >= _elementPosWithOffset[0].x && !_isScrolling
                || _contentRect.anchoredPosition.x <= _elementPosWithOffset[_elementPosWithOffset.Length - 1].x &&
                !_isScrolling)
                scrollRect.inertia = false;

            float nearestPos = float.MaxValue;
            
            for (int i = 0; i < PanCount; i++)
            {
                float distance = Mathf.Abs(_contentRect.anchoredPosition.x - _elementPosWithOffset[i].x);

                if (distance < nearestPos)
                {
                    nearestPos = distance;

                    if (i > ((PanCount - 1) - LastIgnoredElementsCount))
                        _selectedPanID = (PanCount - 1) - LastIgnoredElementsCount;
                    else
                        _selectedPanID = i;
                }
            }

            float scrollVelocity = Mathf.Abs(scrollRect.velocity.x);

            if (scrollVelocity < InertiaDisablingScrollVelocity && !_isScrolling)
                scrollRect.inertia = false;
            if (_isScrolling || scrollVelocity > InertiaDisablingScrollVelocity)
                return;

            _contentVector.x = Mathf.SmoothStep(_contentRect.anchoredPosition.x,
                _elementPosWithOffset[_selectedPanID].x, SnapSpeed * Time.fixedDeltaTime);
            _contentRect.anchoredPosition = _contentVector;
        }


        //Display snapping points
        private void OnDrawGizmos()
        {
            if (_pansPosLocal == null || !IsShowHelpers)
                return;

            for (int i = 0; i < _pansPosLocal.Length; i++)
            {
                Gizmos.color = Color.blue;
                Vector3 gloabalPos = transform.GetChild(i).GetComponent<RectTransform>().position;
                Vector3 gloabalScale = transform.GetChild(i).GetComponent<RectTransform>().sizeDelta;
                Vector3 panPos = new Vector3(gloabalPos.x - (gloabalScale.x / 2 * SnapPointOffset), gloabalPos.y);
                Gizmos.DrawSphere(panPos, 30f);
            }
        }

        //Calculate how many elements that must be ignored to fill the content space.
        private void CalculateLastIndexElement()
        {
            float rectWidth = scrollRect.GetComponent<RectTransform>().sizeDelta.x;
            float elementSpacing = GetComponent<HorizontalLayoutGroup>().spacing;
            
            float currentSizeSumm = 0f;
            int lastElementIndex = 0;
            for (int i = 0; i < _snappingElementsSize.Length; i++)
            {
                currentSizeSumm += _snappingElementsSize[(_snappingElementsSize.Length - 1) - lastElementIndex].x +
                                   elementSpacing;
                if (rectWidth > currentSizeSumm)
                {
                    lastElementIndex++;
                }
                else
                {
                    break;
                }
            }

            LastIgnoredElementsCount = lastElementIndex - 1;
        }
    }
}