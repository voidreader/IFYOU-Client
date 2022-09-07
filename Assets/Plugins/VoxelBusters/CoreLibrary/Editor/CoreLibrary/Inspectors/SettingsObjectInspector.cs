using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public abstract class SettingsObjectInspector : UnityEditor.Editor
    {
        #region Constants

        private     static  readonly    ButtonInfo[]            s_emptyButtonArray          = new ButtonInfo[0];

        private     static  readonly    PropertyGroupInfo[]     s_emptyPropertyGroupArray   = new PropertyGroupInfo[0];

        #endregion

        #region Fields

        private     string                  m_productName;

        private     string                  m_productVersion;

        private     ButtonInfo[]            m_topBarButtons;

        private     PropertyGroupInfo[]     m_propertyGroups;

        private     int                     m_propertyGroupCount;

        private     SerializedProperty      m_activePropertyGroup;

        private     InspectorDrawStyle      m_drawStyle;

        // assets
        private     Texture2D               m_logoIcon;

        private     Texture2D               m_toggleOnIcon;

        private     Texture2D               m_toggleOffIcon;

        #endregion

        #region Properties

        protected GUIStyle HeaderButtonStyle { get; private set; }

        protected GUIStyle HeaderFoldoutStyle { get; private set; }

        protected GUIStyle HeaderLabelStyle { get; private set; }

        #endregion

        #region Unity methods

        protected virtual void OnEnable()
        {
            // set properties
            var     ownerPackage        = GetOwner();
            m_productName               = ownerPackage.DisplayName;
            m_productVersion            = $"v{ownerPackage.Version}";
            m_drawStyle                 = GetDrawStyle();
            m_topBarButtons             = GetTopBarButtons();
            if (m_drawStyle == InspectorDrawStyle.Group)
            {
                m_propertyGroups        = GetPropertyGroups();
                m_propertyGroupCount    = m_propertyGroups.Length;
            }
            LoadAssets();
        }

        public override void OnInspectorGUI()
        {
            EnsureStylesAreLoaded();
            DrawTopBar();
            EditorGUI.BeginChangeCheck();
            switch (m_drawStyle)
            {
                case InspectorDrawStyle.Default:
                    DrawDefaultInspector();
                    break;

                case InspectorDrawStyle.Group:
                    DrawGroupStyleInspector();
                    break;

                case InspectorDrawStyle.Custom:
                    DrawCustomInspector();
                    break;
            }
            GUILayout.Space(5f);
            DrawFooter();
            if (EditorGUI.EndChangeCheck())
            {
                // save changes
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
        }

        #endregion

        #region Abstract methods

        protected abstract UnityPackageDefinition GetOwner();

        #endregion

        #region Getter methods

        protected virtual InspectorDrawStyle GetDrawStyle() => InspectorDrawStyle.Default;

        protected virtual ButtonInfo[] GetTopBarButtons() => s_emptyButtonArray;

        protected virtual PropertyGroupInfo[] GetPropertyGroups() => s_emptyPropertyGroupArray;

        #endregion

        #region Draw methods

        protected virtual void DrawTopBar()
        {
            GUILayout.BeginHorizontal(CustomEditorStyles.GroupBackground);

            // logo section
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
            GUILayout.Label(m_logoIcon, GUILayout.Height(64f), GUILayout.Width(64f));
            GUILayout.Space(2f);
            GUILayout.EndVertical();

            // product info
            GUILayout.BeginVertical();
            GUILayout.Label(m_productName, CustomEditorStyles.Heading1);
            GUILayout.Label(m_productVersion, CustomEditorStyles.Normal);
            GUILayout.Label("Copyright © 2022 Voxel Busters Interactive LLP.", CustomEditorStyles.Options);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // top bar buttons
            if (m_topBarButtons != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                int     buttonCount = m_topBarButtons.Length;
                for (int iter = 0; iter < buttonCount; iter++)
                {
                    var     current = m_topBarButtons[iter];
                    string  style   = "ButtonMid";
                    if (iter == 0)
                    {
                        style       = "ButtonLeft";
                    }
                    else if (iter == (buttonCount - 1))
                    {
                        style       = "ButtonRight";
                    }
                    if (GUILayout.Button(current.Label, style))
                    {
                        current.OnClick?.Invoke();
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        protected virtual void DrawGroupStyleInspector()
        {
            for (int iter = 0; iter < m_propertyGroupCount; iter++)
            {
                var     property    = m_propertyGroups[iter];
                DrawPropertyGroup(property);
            }
        }

        protected virtual void DrawCustomInspector()
        { }

        protected virtual void DrawFooter()
        { }

        protected void DrawPropertyGroup(PropertyGroupInfo propertyGroup)
        {
            var     property        = propertyGroup.Reference;
            EditorGUILayout.BeginVertical(CustomEditorStyles.GroupBackground);
            if (DrawControlHeader(property, propertyGroup.DisplayName))
            {
                bool    oldGUIState         = GUI.enabled;
                var     enabledProperty     = property.FindPropertyRelative("m_isEnabled");

                // update gui state
                GUI.enabled     = (enabledProperty == null) || enabledProperty.boolValue;

                // display child properties
                if (propertyGroup.OnDrawChildProperties != null)
                {
                    propertyGroup.OnDrawChildProperties(property);
                }
                else
                {
                    DrawChildProperties(property, ignoreProperties: "m_isEnabled");
                }

                // reset gui state
                GUI.enabled     = oldGUIState;
            }
            EditorGUILayout.EndVertical();
        }

        private bool DrawControlHeader(SerializedProperty property, string displayName)
        {
            // draw rect
            var     rect                = EditorGUILayout.GetControlRect(false, 30f);
            GUI.Box(rect, GUIContent.none, HeaderButtonStyle);

            // draw foldable control
            bool    isSelected          = (property == m_activePropertyGroup);
            var     foldOutRect         = new Rect(rect.x + 5f, rect.y, 50f, rect.height);
            EditorGUI.LabelField(foldOutRect, isSelected ? "-" : "+", CustomEditorStyles.Heading3);

            // draw label 
            var     labelRect           = new Rect(rect.x + 20f, rect.y, rect.width - 100f, rect.height);
            EditorGUI.LabelField(labelRect, displayName, CustomEditorStyles.Heading3);

            // draw selectable rect
            var     selectableRect      = new Rect(rect.x, rect.y, rect.width - 100f, rect.height);
            if (EditorLayoutUtility.TransparentButton(selectableRect))
            {
                isSelected              = OnPropertyGroupSelect(property);
            }

            // draw toggle button
            var     enabledProperty     = property.FindPropertyRelative("m_isEnabled");
            if ((enabledProperty != null) /*&& NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState()*/)
            {
                Rect    toggleRect                  = new Rect(rect.xMax - 64f, rect.y, 64f, 25f);
                if (GUI.Button(toggleRect, enabledProperty.boolValue ? m_toggleOnIcon : m_toggleOffIcon, CustomEditorStyles.InvisibleButton))
                {
                    enabledProperty.boolValue       = !enabledProperty.boolValue;

#if UNITY_ANDROID
                    //TODO : Fire an event if any feature toggles and listent for adding the dependencies
                    EditorPrefs.SetBool("refresh-feature-dependencies", true);
#endif

                }
                
            }
            return isSelected;
        }

        protected void DrawChildProperties(SerializedProperty property, string prefix = null,
            bool indent = true, params string[] ignoreProperties)
        {
            try
            {
                if (indent)
                {
                    EditorGUI.indentLevel++;
                }

                // move pointer to first element
                var     currentProperty  = property.Copy();
                var     endProperty      = default(SerializedProperty);

                // start iterating through the properties
                bool    firstTime       = true;
                while (currentProperty.NextVisible(enterChildren: firstTime))
                {
                    if (firstTime)
                    {
                        endProperty      = property.GetEndProperty();
                        firstTime        = false;
                    }
                    if (SerializedProperty.EqualContents(currentProperty, endProperty))
                    {
                        break;
                    }

                    // exclude specified properties
                    if ((ignoreProperties != null) && System.Array.Exists(ignoreProperties, (item) => string.Equals(item, currentProperty.name)))
                    {
                        continue;
                    }

                    // display the property
                    if (prefix != null)
                    {
                        EditorGUILayout.PropertyField(currentProperty, new GUIContent($"{prefix} {currentProperty.displayName}", currentProperty.tooltip), true);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(currentProperty, true);
                    }
                }
            }
            finally
            {
                if (indent)
                {
                    EditorGUI.indentLevel--;
                }
            }
        }

        #endregion

        #region Private methods

        protected void EnsureChangesAreSerialized()
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void EnsureStylesAreLoaded()
        {
            // check whether styles are already loaded
            if (null != HeaderButtonStyle) return;

            // set custom style properties
            HeaderButtonStyle   = new GUIStyle("PreButton")
            {
                fixedHeight     = 0,
                fontSize        = 20,
                alignment       = TextAnchor.MiddleLeft,
            };
            HeaderFoldoutStyle  = new GUIStyle("WhiteBoldLabel")
            {
                fontSize        = 20,
                alignment       = TextAnchor.MiddleLeft,
            };
            HeaderLabelStyle    = new GUIStyle("WhiteBoldLabel")
            {
                fontSize        = 20,
                alignment       = TextAnchor.MiddleLeft,
            };
        }

        private void LoadAssets()
        {
            // load custom assets
            var     ownerResourcePath   = GetOwner().GetEditorResourcesPath();
            m_logoIcon                  = AssetDatabase.LoadAssetAtPath<Texture2D>(ownerResourcePath + "/Textures/logo.png");

            // load default assets
            var     commonResourcePath  = CoreLibrarySettings.Package.GetEditorResourcesPath();
            m_toggleOnIcon              = AssetDatabase.LoadAssetAtPath<Texture2D>(commonResourcePath + "/Textures/toggle-on.png");
            m_toggleOffIcon             = AssetDatabase.LoadAssetAtPath<Texture2D>(commonResourcePath + "/Textures/toggle-off.png");
        }

        #endregion

        #region Callback methods

        protected bool OnPropertyGroupSelect(SerializedProperty property)
        {
            var     lastActiveProperty  = m_activePropertyGroup;
            if (m_activePropertyGroup == null)
            {
                property.isExpanded     = true;
                m_activePropertyGroup   = property;

                return true;
            }
            if (m_activePropertyGroup == property)
            {
                property.isExpanded     = false;
                m_activePropertyGroup   = null;

                return false;
            }

            // update reference
            lastActiveProperty.isExpanded       = false;
            m_activePropertyGroup               = property;
            m_activePropertyGroup.isExpanded    = true;

            return true;
        }

        #endregion

        #region Nested types

        protected enum InspectorDrawStyle
        {
            Default = 1,

            Group,

            Custom,
        }

        protected class ButtonInfo
        {
            #region Properties

            public string Label { get; private set; }

            public System.Action OnClick { get; private set; }

            #endregion

            #region Constructors

            public ButtonInfo(string label, System.Action onClick)
            {
                // set properties
                Label       = label;
                OnClick     = onClick;
            }

            #endregion
        }

        protected class PropertyGroupInfo
        {
            #region Properties

            public SerializedProperty Reference { get; private set; }

            public string DisplayName { get; private set; }

            public System.Action<SerializedProperty> OnDrawChildProperties { get; private set; }

            #endregion

            #region Constructors

            public PropertyGroupInfo(SerializedProperty reference, string displayName,
                System.Action<SerializedProperty> onDrawChildProperties = null)
            {
                // set properties
                Reference                   = reference;
                DisplayName                 = displayName;
                OnDrawChildProperties       = onDrawChildProperties;
            }

            #endregion
        }

        #endregion
    }
}