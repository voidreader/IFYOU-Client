// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Layouts
{
    [RequireComponent(typeof(RectTransform))]
    public class UIBehaviourHandler : UnityEngine.EventSystems.UIBehaviour
    {
        public RectTransform rectTransform { get; private set; }
        public LayoutGroup layoutGroup { get; private set; }
        public UnityEvent OnRectTransformDimensionsChanged;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
            layoutGroup = GetComponent<LayoutGroup>();
            RefreshLayout();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            OnRectTransformDimensionsChanged?.Invoke();
        }

        public void RefreshLayout()
        {
            if (layoutGroup != null) return;
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        public void ForceRebuildLayoutImmediate() =>
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        public void MarkLayoutForRebuild() =>
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }
}
