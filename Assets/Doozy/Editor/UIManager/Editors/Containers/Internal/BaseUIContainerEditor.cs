// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Containers.Internal
{
    public abstract class BaseUIContainerEditor : UnityEditor.Editor
    {
        public static IEnumerable<Texture2D> arrowDownIconTextures => EditorSpriteSheets.EditorUI.Arrows.ArrowDown;
        public static IEnumerable<Texture2D> arrowUpIconTextures => EditorSpriteSheets.EditorUI.Arrows.ArrowUp;
        public static IEnumerable<Texture2D> hideIconTextures => EditorSpriteSheets.EditorUI.Icons.Hide;
        public static IEnumerable<Texture2D> resetIconTextures => EditorSpriteSheets.EditorUI.Icons.Reset;
        public static IEnumerable<Texture2D> settingsIconTextures => EditorSpriteSheets.EditorUI.Icons.Settings;
        public static IEnumerable<Texture2D> showIconTextures => EditorSpriteSheets.EditorUI.Icons.Show;
        public static IEnumerable<Texture2D> unityIconTextures => EditorSpriteSheets.EditorUI.Icons.UnityEvent;

        protected virtual Color accentColor => EditorColors.UIManager.UIComponent;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        protected UIContainer uiContainer => (UIContainer)target;
        protected IEnumerable<UIContainer> uiContainers => targets.Cast<UIContainer>();

        protected ModyEvent onShowCallback => uiContainer.OnShowCallback;
        protected ModyEvent onVisibleCallback => uiContainer.OnVisibleCallback;
        protected ModyEvent onHideCallback => uiContainer.OnHideCallback;
        protected ModyEvent onHiddenCallback => uiContainer.OnHiddenCallback;
        protected VisibilityStateEvent onVisibilityChangedCallback => uiContainer.OnVisibilityChangedCallback;

        protected bool hasOnShowCallback => onShowCallback is { Enabled: true } && onShowCallback.hasEvents | onShowCallback.hasRunners;
        protected bool hasOnVisibleCallback => onVisibleCallback is { Enabled: true } && onVisibleCallback.hasEvents | onVisibleCallback.hasRunners;
        protected bool hasOnHideCallback => onHideCallback is { Enabled: true } && onHideCallback.hasEvents | onHideCallback.hasRunners;
        protected bool hasOnHiddenCallback => onHiddenCallback is { Enabled: true } && onHiddenCallback.hasEvents | onHiddenCallback.hasRunners;
        protected bool hasOnVisibilityChangedCallback => onVisibilityChangedCallback != null && onVisibilityChangedCallback.GetPersistentEventCount() > 0;
        protected bool hasCallbacks => hasOnShowCallback | hasOnVisibleCallback | hasOnHideCallback | hasOnHiddenCallback | hasOnVisibilityChangedCallback;

        protected bool hasShowProgressors => propertyShowProgressors.arraySize > 0;
        protected bool hasHideProgressors => propertyHideProgressors.arraySize > 0;
        protected bool hasShowHideProgressors => propertyShowHideProgressors.arraySize > 0;
        protected bool hasProgressors => hasShowProgressors | hasHideProgressors | hasShowHideProgressors;

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }

        protected EnabledIndicator callbacksTabIndicator { get; set; }
        protected EnabledIndicator progressorsTabIndicator { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        protected FluidAnimatedContainer progressorsAnimatedContainer { get; set; }

        protected FluidButton getCustomPositionButton { get; set; }
        protected FluidButton resetCustomPositionButton { get; set; }
        protected FluidButton setCustomPositionButton { get; set; }

        protected FluidField autoHideAfterShowFluidField { get; set; }
        protected FluidField customStartPositionFluidField { get; set; }
        protected FluidField onStartBehaviourFluidField { get; set; }
        protected FluidField whenHiddenFluidField { get; set; }
        protected FluidField clearSelectedFluidField { get; set; }
        protected FluidField autoSelectAfterShowFluidField { get; set; }

        protected FluidToggleButtonTab settingsTabButton { get; set; }
        protected FluidToggleButtonTab callbacksTabButton { get; set; }
        protected FluidToggleButtonTab progressorsTabButton { get; set; }

        protected FluidToggleGroup tabsGroup { get; set; }

        protected FluidToggleSwitch autoHideAfterShowSwitch { get; set; }
        protected FluidToggleSwitch disableCanvasWhenHiddenSwitch { get; set; }
        protected FluidToggleSwitch disableGameObjectWhenHiddenSwitch { get; set; }
        protected FluidToggleSwitch disableGraphicRaycasterWhenHiddenSwitch { get; set; }
        protected FluidToggleSwitch useCustomStartPositionSwitch { get; set; }
        protected FluidToggleSwitch clearSelectedOnHideSwitch { get; set; }
        protected FluidToggleSwitch clearSelectedOnShowSwitch { get; set; }
        protected FluidToggleSwitch autoSelectAfterShowSwitch { get; set; }

        protected ObjectField autoSelectTargetObjectField { get; set; }

        protected VisualElement callbacksTab { get; set; }
        protected VisualElement progressorsTab { get; set; }

        protected SerializedProperty propertyOnStartBehaviour { get; set; }
        protected SerializedProperty propertyOnShowCallback { get; set; }
        protected SerializedProperty propertyOnVisibleCallback { get; set; }
        protected SerializedProperty propertyOnHideCallback { get; set; }
        protected SerializedProperty propertyOnHiddenCallback { get; set; }
        protected SerializedProperty propertyOnVisibilityChangedCallback { get; set; }
        protected SerializedProperty propertyShowProgressors { get; set; }
        protected SerializedProperty propertyHideProgressors { get; set; }
        protected SerializedProperty propertyShowHideProgressors { get; set; }
        protected SerializedProperty propertyCustomStartPosition { get; set; }
        protected SerializedProperty propertyUseCustomStartPosition { get; set; }
        protected SerializedProperty propertyAutoHideAfterShow { get; set; }
        protected SerializedProperty propertyAutoHideAfterShowDelay { get; set; }
        protected SerializedProperty propertyDisableGameObjectWhenHidden { get; set; }
        protected SerializedProperty propertyDisableCanvasWhenHidden { get; set; }
        protected SerializedProperty propertyDisableGraphicRaycasterWhenHidden { get; set; }
        protected SerializedProperty propertyClearSelectedOnHide { get; set; }
        protected SerializedProperty propertyClearSelectedOnShow { get; set; }
        protected SerializedProperty propertyAutoSelectAfterShow { get; set; }
        protected SerializedProperty propertyAutoSelectTarget { get; set; }

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();

            callbacksTabIndicator?.Recycle();

            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            progressorsAnimatedContainer?.Dispose();

            getCustomPositionButton?.Recycle();
            resetCustomPositionButton?.Recycle();
            setCustomPositionButton?.Recycle();

            tabsGroup?.Recycle();

            autoHideAfterShowFluidField?.Recycle();
            customStartPositionFluidField?.Recycle();
            onStartBehaviourFluidField?.Recycle();
            whenHiddenFluidField?.Recycle();
            clearSelectedFluidField?.Recycle();
            autoSelectAfterShowFluidField?.Recycle();

            settingsTabButton?.Recycle();
            callbacksTabButton?.Recycle();

            autoHideAfterShowSwitch?.Recycle();
            disableCanvasWhenHiddenSwitch?.Recycle();
            disableGameObjectWhenHiddenSwitch?.Recycle();
            disableGraphicRaycasterWhenHiddenSwitch?.Recycle();
            useCustomStartPositionSwitch?.Recycle();
            clearSelectedOnHideSwitch?.Recycle();
            clearSelectedOnShowSwitch?.Recycle();
            autoSelectAfterShowSwitch?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void FindProperties()
        {
            propertyOnStartBehaviour = serializedObject.FindProperty(nameof(UIContainer.OnStartBehaviour));
            propertyOnShowCallback = serializedObject.FindProperty(nameof(UIContainer.OnShowCallback));
            propertyOnVisibleCallback = serializedObject.FindProperty(nameof(UIContainer.OnVisibleCallback));
            propertyOnHideCallback = serializedObject.FindProperty(nameof(UIContainer.OnHideCallback));
            propertyOnHiddenCallback = serializedObject.FindProperty(nameof(UIContainer.OnHiddenCallback));
            propertyOnVisibilityChangedCallback = serializedObject.FindProperty(nameof(UIContainer.OnVisibilityChangedCallback));
            propertyShowProgressors = serializedObject.FindProperty("ShowProgressors");
            propertyHideProgressors = serializedObject.FindProperty("HideProgressors");
            propertyShowHideProgressors = serializedObject.FindProperty("ShowHideProgressors");
            propertyCustomStartPosition = serializedObject.FindProperty(nameof(UIContainer.CustomStartPosition));
            propertyUseCustomStartPosition = serializedObject.FindProperty(nameof(UIContainer.UseCustomStartPosition));
            propertyAutoHideAfterShow = serializedObject.FindProperty(nameof(UIContainer.AutoHideAfterShow));
            propertyAutoHideAfterShowDelay = serializedObject.FindProperty(nameof(UIContainer.AutoHideAfterShowDelay));
            propertyDisableGameObjectWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableGameObjectWhenHidden));
            propertyDisableCanvasWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableCanvasWhenHidden));
            propertyDisableGraphicRaycasterWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableGraphicRaycasterWhenHidden));
            propertyClearSelectedOnHide = serializedObject.FindProperty(nameof(UIContainer.ClearSelectedOnHide));
            propertyClearSelectedOnShow = serializedObject.FindProperty(nameof(UIContainer.ClearSelectedOnShow));
            propertyAutoSelectAfterShow = serializedObject.FindProperty(nameof(UIContainer.AutoSelectAfterShow));
            propertyAutoSelectTarget = serializedObject.FindProperty(nameof(UIContainer.AutoSelectTarget));
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Large);

            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", false).Show(false);
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            progressorsAnimatedContainer = new FluidAnimatedContainer("Progressors", true).Hide(false);

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            InitializeSettings();
            InitializeCallbacks();
            InitializeProgressors();
        }

        protected virtual void InitializeSettings()
        {
            settingsTabButton =
                DesignUtils.GetTabButtonForComponentSection(settingsIconTextures, selectableAccentColor)
                    .SetLabelText("Settings")
                    .SetIsOn(true, false)
                    .SetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue));

            settingsTabButton.AddToToggleGroup(tabsGroup);

            InitializeStartBehaviour();
            InitializeAutoHideAfterShow();
            InitializeCustomStartPosition();
            InitializeWhenHidden();
            InitializeSelected();
        }

        protected void InitializeStartBehaviour()
        {
            EnumField onStartBehaviourEnumField = DesignUtils.NewEnumField(propertyOnStartBehaviour.propertyPath).SetStyleFlexGrow(1);
            onStartBehaviourEnumField.SetTooltip(GetContainerBehaviourTooltip((ContainerBehaviour)propertyOnStartBehaviour.enumValueIndex));
            onStartBehaviourEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                onStartBehaviourEnumField.SetTooltip(GetContainerBehaviourTooltip((ContainerBehaviour)evt.newValue));
            });
            onStartBehaviourFluidField = FluidField.Get("OnStart Behaviour").AddFieldContent(onStartBehaviourEnumField);
            onStartBehaviourFluidField.fieldContent.SetStyleFlexGrow(0);

            string GetContainerBehaviourTooltip(ContainerBehaviour behaviour)
            {
                switch (behaviour)
                {
                    case ContainerBehaviour.Disabled: return "Do nothing";
                    case ContainerBehaviour.InstantHide: return "Instant Hide (no animation)";
                    case ContainerBehaviour.InstantShow: return "Instant Show (no animation)";
                    case ContainerBehaviour.Hide: return "Hide (animated)";
                    case ContainerBehaviour.Show: return "Show (animated)";
                    default: throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
                }
            }
        }

        protected void InitializeAutoHideAfterShow()
        {
            Label hideDelayLabel =
                DesignUtils.NewLabel("Auto Hide Delay")
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            FloatField hideDelayPropertyField =
                DesignUtils.NewFloatField(propertyAutoHideAfterShowDelay)
                    .SetTooltip("Time interval after which Hide is triggered")
                    .SetStyleWidth(40)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            Label secondsLabel = DesignUtils.NewLabel("seconds");

            hideDelayLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);
            hideDelayPropertyField.SetEnabled(propertyAutoHideAfterShow.boolValue);
            secondsLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);

            autoHideAfterShowSwitch =
                FluidToggleSwitch.Get()
                    .BindToProperty(propertyAutoHideAfterShow)
                    .SetTooltip("If TRUE, after Show, Hide it will get automatically triggered after the AutoHideAfterShowDelay time interval has passed")
                    .SetOnValueChanged(evt =>
                    {
                        if (evt?.newValue == null) return;
                        hideDelayLabel.SetEnabled(evt.newValue);
                        hideDelayPropertyField.SetEnabled(evt.newValue);
                        secondsLabel.SetEnabled(evt.newValue);

                    })
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            autoHideAfterShowFluidField =
                FluidField.Get("Auto Hide after Show")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(autoHideAfterShowSwitch)
                            .AddChild(hideDelayLabel)
                            .AddChild(hideDelayPropertyField)
                            .AddChild(secondsLabel)
                            .AddChild(DesignUtils.flexibleSpace)
                    );
        }

        protected void InitializeCustomStartPosition()
        {
            PropertyField customStartPositionPropertyField =
                DesignUtils.NewPropertyField(propertyCustomStartPosition)
                    .TryToHideLabel()
                    .SetTooltip("AnchoredPosition3D to snap to on Awake")
                    .SetStyleAlignSelf(Align.Center);

            useCustomStartPositionSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertyUseCustomStartPosition)
                    .SetTooltip("If TRUE, this view will 'snap' to the custom start position on Awake");

            FluidButton GetButton(IEnumerable<Texture2D> iconTextures, string labelText, string tooltip) =>
                FluidButton.Get()
                    .SetIcon(iconTextures)
                    .SetLabelText(labelText)
                    .SetTooltip(tooltip)
                    .SetElementSize(ElementSize.Tiny)
                    .SetButtonStyle(ButtonStyle.Contained);

            getCustomPositionButton =
                GetButton(arrowDownIconTextures, "Get", "Set the current RectTransform anchoredPosition3D as the custom start position")
                    .SetOnClick(() =>
                    {
                        propertyCustomStartPosition.vector3Value = uiContainer.rectTransform.anchoredPosition3D;
                        serializedObject.ApplyModifiedProperties();
                    });

            setCustomPositionButton =
                GetButton(arrowUpIconTextures, "Set", "Snap the RectTransform current anchoredPosition3D to the set custom start position")
                    .SetOnClick(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            // ReSharper disable once CoVariantArrayConversion
                            Undo.RecordObjects(uiContainers.Select(ct => ct.rectTransform).ToArray(), "Set Position");
                            foreach (UIContainer container in uiContainers)
                                container.rectTransform.anchoredPosition3D = container.CustomStartPosition;
                            return;
                        }

                        Undo.RecordObject(uiContainer.rectTransform, "Set Position");
                        uiContainer.rectTransform.anchoredPosition3D = uiContainer.CustomStartPosition;
                    });

            resetCustomPositionButton =
                GetButton(resetIconTextures, "Reset", "Reset the custom start position to (0,0,0)")
                    .SetOnClick(() =>
                    {
                        propertyCustomStartPosition.vector3Value = Vector3.zero;
                        serializedObject.ApplyModifiedProperties();
                    });

            customStartPositionFluidField =
                FluidField.Get("Custom Start Position")
                    .AddFieldContent
                    (
                        DesignUtils.row.SetStyleAlignItems(Align.Center)
                            .AddChild(useCustomStartPositionSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(customStartPositionPropertyField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(getCustomPositionButton)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(setCustomPositionButton)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(resetCustomPositionButton)
                    );

            customStartPositionPropertyField.SetEnabled(propertyUseCustomStartPosition.boolValue);
            getCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);
            setCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);
            resetCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);

            useCustomStartPositionSwitch.SetOnValueChanged(callback: evt =>
            {
                customStartPositionPropertyField.SetEnabled(evt.newValue);
                getCustomPositionButton.SetEnabled(evt.newValue);
                setCustomPositionButton.SetEnabled(evt.newValue);
                resetCustomPositionButton.SetEnabled(evt.newValue);
            });
        }

        protected void InitializeWhenHidden()
        {

            disableGameObjectWhenHiddenSwitch =
                FluidToggleSwitch.Get("GameObject")
                    .BindToProperty(propertyDisableGameObjectWhenHidden)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, after Hide, the GameObject this component is attached to, will be disabled");

            disableCanvasWhenHiddenSwitch =
                FluidToggleSwitch.Get("Canvas")
                    .BindToProperty(propertyDisableCanvasWhenHidden)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, after Hide, the Canvas component found on the same GameObject this component is attached to, will be disabled");

            disableGraphicRaycasterWhenHiddenSwitch =
                FluidToggleSwitch.Get("GraphicRaycaster")
                    .BindToProperty(propertyDisableGraphicRaycasterWhenHidden)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, after Hide, the GraphicRaycaster component found on the same GameObject this component is attached to, will be disabled");

            whenHiddenFluidField = FluidField.Get("When Hidden, disable")
                .AddFieldContent
                (
                    DesignUtils.row
                        .AddChild(disableGameObjectWhenHiddenSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(disableCanvasWhenHiddenSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(disableGraphicRaycasterWhenHiddenSwitch)
                        .AddChild(DesignUtils.flexibleSpace)
                );
        }

        protected void InitializeSelected()
        {
            clearSelectedOnShowSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Show")
                    .SetTooltip("If TRUE, when this container is shown, any GameObject that is selected by the EventSystem.current will get deselected")
                    .BindToProperty(propertyClearSelectedOnShow)
                    .SetToggleAccentColor(selectableAccentColor);

            clearSelectedOnHideSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Hide")
                    .SetTooltip("If TRUE, when this container is hidden, any GameObject that is selected by the EventSystem.current will get deselected")
                    .BindToProperty(propertyClearSelectedOnHide)
                    .SetToggleAccentColor(selectableAccentColor);

            clearSelectedFluidField =
                FluidField.Get()
                    .SetLabelText("Clear Selected on")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(clearSelectedOnShowSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(clearSelectedOnHideSwitch)
                    )
                    .SetStyleMinWidth(150);

            autoSelectAfterShowSwitch =
                FluidToggleSwitch.Get()
                    .SetTooltip("If TRUE, after this container has been shown, the referenced selectable GameObject will get automatically selected by EventSystem.current")
                    .BindToProperty(propertyAutoSelectAfterShow)
                    .SetToggleAccentColor(selectableAccentColor);

            autoSelectTargetObjectField =
                DesignUtils.NewObjectField(propertyAutoSelectTarget, typeof(GameObject))
                    .SetTooltip("Reference to the GameObject that should be selected after this view has been shown. Works only if AutoSelectAfterShow is TRUE");

            autoSelectTargetObjectField.SetEnabled(propertyAutoSelectAfterShow.boolValue);
            autoSelectAfterShowSwitch.SetOnValueChanged(evt =>
            {
                if (evt?.newValue == null) return;
                autoSelectTargetObjectField.SetEnabled(evt.newValue);
            });

            autoSelectAfterShowFluidField =
                FluidField.Get()
                    .SetLabelText("Auto select GameObject after Show")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(autoSelectAfterShowSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(autoSelectTargetObjectField)
                    );
        }

        protected virtual void InitializeProgressors()
        {
            (progressorsTabButton, progressorsTabIndicator, progressorsTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator
                (
                    EditorSpriteSheets.Reactor.Icons.Progressor,
                    EditorSelectableColors.Reactor.Red,
                    EditorColors.Reactor.Red
                );

            progressorsAnimatedContainer.SetOnShowCallback(() =>
            {
                progressorsAnimatedContainer
                    .AddContent
                    (
                        GetProgressorsListView
                        (
                            propertyShowProgressors,
                            "Show Progressors",
                            "Progressors triggered on Show. Plays forward on Show."
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock4X)
                    .AddContent
                    (
                        GetProgressorsListView
                        (
                            propertyHideProgressors,
                            "Hide Progressors",
                            "Progressors triggered on Hide. Plays forward on Hide."
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock4X)
                    .AddContent
                    (
                        GetProgressorsListView
                        (
                            propertyShowHideProgressors,
                            "Show/Hide Progressors",
                            "Progressors triggered on both Show and Hide. Plays forward on Show and in reverse on Hide."
                        )
                    );

                progressorsAnimatedContainer.Bind(serializedObject);
            });

            root.schedule
                .Execute
                (
                    () => progressorsTabIndicator.Toggle(hasProgressors, true)
                )
                .Every(250);

            progressorsTabButton
                .SetLabelText("Progressors")
                .SetOnValueChanged(evt => progressorsAnimatedContainer.Toggle(evt.newValue))
                .AddToToggleGroup(tabsGroup);
        }

        private static FluidListView GetProgressorsListView(SerializedProperty arrayProperty, string listTitle, string listDescription) =>
            DesignUtils.GetObjectListView(arrayProperty, listTitle, listDescription, typeof(Progressor));

        protected virtual void InitializeCallbacks()
        {
            (callbacksTabButton, callbacksTabIndicator, callbacksTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator
                (
                    unityIconTextures,
                    DesignUtils.callbackSelectableColor,
                    DesignUtils.callbacksColor
                );
            
            FluidField GetCallbackField(IEnumerable<Texture2D> icon, string labelText, SerializedProperty property) =>
                FluidField.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetIcon(icon)
                    .SetLabelText(labelText)
                    .AddFieldContent(DesignUtils.NewPropertyField(property));

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent
                    (
                        GetCallbackField
                        (
                            EditorSpriteSheets.EditorUI.Icons.Show,
                            "Show animation started",
                            propertyOnShowCallback
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        GetCallbackField
                        (
                            EditorSpriteSheets.EditorUI.Icons.Show,
                            "Visible - Show animation finished",
                            propertyOnVisibleCallback
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        GetCallbackField
                        (
                            EditorSpriteSheets.EditorUI.Icons.Hide,
                            "Hide animation started",
                            propertyOnHideCallback
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        GetCallbackField
                        (
                            EditorSpriteSheets.EditorUI.Icons.Hide,
                            "Hidden - Hide animation finished",
                            propertyOnHiddenCallback
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        GetCallbackField
                        (
                            EditorSpriteSheets.EditorUI.Icons.VisibilityChanged,
                            "Visibility changed",
                            propertyOnVisibilityChangedCallback
                        )
                    )
                    ;

                callbacksAnimatedContainer.Bind(serializedObject);
            });

            root.schedule
                .Execute
                (
                    () => callbacksTabIndicator.Toggle(hasCallbacks, true)
                )
                .Every(250);

            callbacksTabIndicator.Toggle(hasCallbacks, false);

            callbacksTabButton
                .SetLabelText("Callbacks")
                .SetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue))
                .AddToToggleGroup(tabsGroup);
        }

        protected VisualElement GetRuntimeControls()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return new VisualElement().SetStyleDisplay(DisplayStyle.None);

            FluidField field = FluidField.Get();
            VisualElement row = DesignUtils.row;
            field.AddFieldContent(row);

            var instantAnimationSwitch = FluidToggleSwitch.Get("Instant");

            FluidButton GetButton() =>
                FluidButton.Get()
                    .SetElementSize(ElementSize.Small)
                    .SetButtonStyle(ButtonStyle.Contained);

            FluidButton showButton = GetButton()
                .SetLabelText("Show")
                .SetIcon(showIconTextures)
                .SetOnClick(() =>
                {
                    bool instantAnimation = instantAnimationSwitch.isOn;

                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIContainer container in uiContainers)
                        {
                            if (instantAnimation)
                            {
                                container.InstantShow();
                                continue;
                            }
                            container.Show();
                        }
                        return;
                    }

                    if (instantAnimation)
                    {
                        uiContainer.InstantShow();
                        return;
                    }

                    uiContainer.Show();

                });

            FluidButton hideButton = GetButton()
                .SetLabelText("Hide")
                .SetIcon(hideIconTextures)
                .SetOnClick(() =>
                {
                    bool instantAnimation = instantAnimationSwitch.isOn;

                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIContainer container in uiContainers)
                        {
                            if (instantAnimation)
                            {
                                container.InstantHide();
                                continue;
                            }
                            container.Hide();
                        }
                        return;
                    }

                    if (instantAnimation)
                    {
                        uiContainer.InstantHide();
                        return;
                    }
                    uiContainer.Hide();

                });

            row
                .AddChild(showButton)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(hideButton)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(instantAnimationSwitch);

            return field
                .SetStyleMarginBottom(DesignUtils.k_Spacing);
        }

        protected virtual void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(settingsTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(callbacksTab)
                        .AddChild(DesignUtils.spaceBlock4X)
                        .AddChild(progressorsTab)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild
                        (
                            DesignUtils.SystemButton_SortComponents
                            (
                                uiContainer.gameObject,
                                nameof(RectTransform),
                                nameof(Canvas),
                                nameof(CanvasGroup),
                                nameof(GraphicRaycaster),
                                nameof(UIContainer)
                            )
                        )
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetRuntimeControls())
                .AddChild
                (
                    settingsAnimatedContainer
                        .AddContent
                        (
                            DesignUtils.column
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(onStartBehaviourFluidField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(autoHideAfterShowFluidField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(customStartPositionFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(whenHiddenFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(clearSelectedFluidField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(autoSelectAfterShowFluidField)
                                )
                                .AddChild(DesignUtils.spaceBlock)
                        )
                )
                .AddChild(callbacksAnimatedContainer)
                .AddChild(progressorsAnimatedContainer)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
