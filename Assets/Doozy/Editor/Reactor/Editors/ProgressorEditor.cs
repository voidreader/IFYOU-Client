// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors
{
    [CustomEditor(typeof(Progressor), true)]
    [CanEditMultipleObjects]
    public class ProgressorEditor : UnityEditor.Editor
    {
        public Progressor castedTarget => (Progressor)target;
        public IEnumerable<Progressor> castedTargets => targets.Cast<Progressor>();

        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private static IEnumerable<Texture2D> unityIconTextures => EditorSpriteSheets.EditorUI.Icons.UnityEvent;
        private static IEnumerable<Texture2D> settingsIconTextures => EditorSpriteSheets.EditorUI.Icons.Settings;
        private static IEnumerable<Texture2D> progressorIconTextures => EditorSpriteSheets.Reactor.Icons.Progressor;

        private bool hasOnValueChangedCallback => castedTarget != null && castedTarget.OnValueChanged?.GetPersistentEventCount() > 0;
        private bool hasOnProgressChangedCallback => castedTarget != null && castedTarget.OnProgressChanged?.GetPersistentEventCount() > 0;
        private bool hasCallbacks => hasOnValueChangedCallback | hasOnProgressChangedCallback;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private ReactionControls controls { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }

        private FluidToggleButtonTab settingsTabButton { get; set; }
        private FluidToggleButtonTab callbacksTabButton { get; set; }
        private FluidToggleButtonTab progressTargetsTabButton { get; set; }
        private FluidToggleButtonTab progressorTargetsTabButton { get; set; }

        private VisualElement callbacksTab { get; set; }
        private VisualElement progressTargetsTab { get; set; }
        private VisualElement progressorTargetsTab { get; set; }

        private EnabledIndicator callbacksTabIndicator { get; set; }
        private EnabledIndicator progressTargetsTabIndicator { get; set; }
        private EnabledIndicator progressorTargetsTabIndicator { get; set; }

        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer targetsContainer { get; set; }
        private FluidAnimatedContainer progressorTargetsContainer { get; set; }

        private SerializedProperty propertyProgressTargets { get; set; }
        private SerializedProperty propertyProgressorTargets { get; set; }
        private SerializedProperty propertyFromValue { get; set; }
        private SerializedProperty propertyToValue { get; set; }
        private SerializedProperty propertyCustomResetValue { get; set; }
        private SerializedProperty propertyReaction { get; set; }
        private SerializedProperty propertyResetValueOnEnable { get; set; }
        private SerializedProperty propertyResetValueOnDisable { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertyOnProgressChanged { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            controls?.Dispose();

            tabsGroup?.Recycle();

            settingsTabButton?.Recycle();
            callbacksTabButton?.Recycle();
            progressTargetsTabButton?.Recycle();
            progressorTargetsTabButton?.Recycle();

            callbacksTabIndicator?.Recycle();
            progressTargetsTabIndicator?.Recycle();
            progressorTargetsTabIndicator?.Recycle();

            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            targetsContainer?.Dispose();
            progressorTargetsContainer?.Dispose();
        }

        private void FindProperties()
        {
            propertyProgressTargets = serializedObject.FindProperty("ProgressTargets");
            propertyProgressorTargets = serializedObject.FindProperty("ProgressorTargets");
            propertyFromValue = serializedObject.FindProperty("FromValue");
            propertyToValue = serializedObject.FindProperty("ToValue");
            propertyCustomResetValue = serializedObject.FindProperty("CustomResetValue");
            propertyReaction = serializedObject.FindProperty("Reaction");
            propertyResetValueOnEnable = serializedObject.FindProperty("ResetValueOnEnable");
            propertyResetValueOnDisable = serializedObject.FindProperty("ResetValueOnDisable");
            propertyOnValueChanged = serializedObject.FindProperty("OnValueChanged");
            propertyOnProgressChanged = serializedObject.FindProperty("OnProgressChanged");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetComponentNameText(nameof(Progressor))
                .SetIcon(progressorIconTextures.ToList())
                .SetElementSize(ElementSize.Large)
                .AddManualButton()
                .AddYouTubeButton();

            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", false).Show(false);
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            targetsContainer = new FluidAnimatedContainer("Progress Targets", true).Hide(false);
            progressorTargetsContainer = new FluidAnimatedContainer("Progressor Targets", true).Hide(false);

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            InitializeSettings();
            InitializeCallbacks();
            InitializeProgressTargets();
            InitializeProgressorTargets();
        }

        private void InitializeSettings()
        {
            settingsTabButton =
                DesignUtils.GetTabButtonForComponentSection(settingsIconTextures, selectableAccentColor)
                    .SetLabelText("Settings")
                    .SetIsOn(true, false)
                    .SetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue));

            settingsTabButton.AddToToggleGroup(tabsGroup);

            FluidField fromValueFluidField = FluidField.Get<FloatField>("FromValue", "From Value");
            FluidField toValueFluidField = FluidField.Get<FloatField>("ToValue", "To Value");
            EnumField resetValueOnEnableEnumField = DesignUtils.NewEnumField(propertyResetValueOnEnable).SetStyleWidth(120);
            FluidField resetValueOnEnableFluidField = FluidField.Get("OnEnable reset value to").AddFieldContent(resetValueOnEnableEnumField).SetStyleFlexGrow(0);
            FluidField customResetValueFluidField = FluidField.Get<FloatField>("CustomResetValue", "Custom Reset Value");
            FluidField reactionFluidField = FluidField.Get<PropertyField>("Reaction.Settings");
            resetValueOnEnableEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                customResetValueFluidField.SetEnabled((ResetValue)evt.newValue == ResetValue.CustomValue);
            });
            root.schedule.Execute(() =>
            {
                if (customResetValueFluidField == null) return;
                if (resetValueOnEnableEnumField?.value == null) return;
                customResetValueFluidField.SetEnabled((ResetValue)resetValueOnEnableEnumField.value == ResetValue.CustomValue);
            });

            settingsAnimatedContainer
                .fluidContainer
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(fromValueFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(toValueFluidField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(reactionFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(resetValueOnEnableFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(customResetValueFluidField)
                )
                ;
        }

        private void InitializeCallbacks()
        {
            (callbacksTabButton, callbacksTabIndicator, callbacksTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator
                (
                    unityIconTextures,
                    DesignUtils.callbackSelectableColor,
                    DesignUtils.callbacksColor
                );

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                            .SetLabelText("Value Changed - [From, To]")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnValueChanged))
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                            .SetLabelText("Progress Changed - [0, 1]")
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnProgressChanged))
                    );

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

        private void InitializeProgressTargets()
        {
            (progressTargetsTabButton, progressTargetsTabIndicator, progressTargetsTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator
                (
                    EditorSpriteSheets.Reactor.Icons.ProgressTarget,
                    EditorSelectableColors.Reactor.Red,
                    EditorColors.Reactor.Red
                );

            targetsContainer.SetOnShowCallback(() =>
            {
                targetsContainer
                    .AddContent
                    (
                        DesignUtils.GetObjectListView
                        (
                            propertyProgressTargets,
                            "Progress Targets",
                            "Progress targets that get updated by this Progressor when activated",
                            typeof(ProgressTarget)
                        )
                    );
                targetsContainer.Bind(serializedObject);
            });

            root.schedule
                .Execute
                (
                    () => progressTargetsTabIndicator.Toggle(propertyProgressTargets.arraySize > 0, true)
                )
                .Every(250);

            progressTargetsTabButton
                .SetLabelText("Targets")
                .SetOnValueChanged(evt => targetsContainer.Toggle(evt.newValue))
                .AddToToggleGroup(tabsGroup);
        }

        private void InitializeProgressorTargets()
        {
            (progressorTargetsTabButton, progressorTargetsTabIndicator, progressorTargetsTab) =
                DesignUtils.GetTabButtonForComponentSectionWithEnabledIndicator
                (
                    EditorSpriteSheets.Reactor.Icons.Progressor,
                    EditorSelectableColors.Reactor.Red,
                    EditorColors.Reactor.Red
                );

            progressorTargetsContainer.SetOnShowCallback(() =>
            {
                progressorTargetsContainer
                    .AddContent
                    (
                        DesignUtils.GetObjectListView
                        (
                            propertyProgressorTargets,
                            "Progressor Targets",
                            "Other Progressors that get updated by this Progressor when activated",
                            typeof(Progressor)
                        )
                    );
                progressorTargetsContainer.Bind(serializedObject);
            });

            root.schedule
                .Execute
                (
                    () => progressorTargetsTabIndicator.Toggle(propertyProgressorTargets.arraySize > 0, true)
                )
                .Every(250);

            progressorTargetsTabButton
                .SetLabelText("Progressors")
                .SetOnValueChanged(evt => progressorTargetsContainer.Toggle(evt.newValue))
                .AddToToggleGroup(tabsGroup);
        }

        protected VisualElement GetRuntimeControls()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return new VisualElement().SetStyleDisplay(DisplayStyle.None);

            controls =
                new ReactionControls()
                    .SetStyleMarginBottom(DesignUtils.k_Spacing)
                    .SetFirstFrameButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.SetProgressAtZero();
                            return;
                        }
                        castedTarget.SetProgressAtZero();
                    })
                    .SetPlayForwardButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Play(PlayDirection.Forward);
                            return;
                        }
                        castedTarget.Play(PlayDirection.Forward);
                    })
                    .SetStopButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Stop();
                            return;
                        }
                        castedTarget.Stop();
                    })
                    .SetPlayReverseButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Play(PlayDirection.Reverse);
                            return;
                        }
                        castedTarget.Play(PlayDirection.Reverse);
                    })
                    .SetReverseButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.Reverse();
                            return;
                        }
                        castedTarget.Reverse();
                    })
                    .SetLastFrameButtonCallback(() =>
                    {
                        if (serializedObject.isEditingMultipleObjects)
                        {
                            foreach (Progressor progressor in castedTargets)
                                progressor.SetProgressAtOne();
                            return;
                        }
                        castedTarget.SetProgressAtOne();
                    });

            return controls;
        }

        private void Compose()
        {
            root.AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(settingsTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(callbacksTab)
                        .AddChild(DesignUtils.spaceBlock4X)
                        .AddChild(progressTargetsTab)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(progressorTargetsTab)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetRuntimeControls())
                .AddChild(settingsAnimatedContainer)
                .AddChild(callbacksAnimatedContainer)
                .AddChild(targetsContainer)
                .AddChild(progressorTargetsContainer)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
