using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public static class CustomEditorStyles
    {
        #region Static properties

        public static GUIStyle Heading1 { get; private set; }

        public static GUIStyle Heading2 { get; private set; }

        public static GUIStyle Heading3 { get; private set; }

        public static GUIStyle Normal { get; private set; }

        public static GUIStyle Options { get; private set; }

        public static GUIStyle Button { get; private set; }

        public static GUIStyle InvisibleButton { get; private set; }

        public static GUIStyle Link { get; private set; }

        public static GUIStyle ItemBackground { get; private set; }

        public static GUIStyle GroupBackground { get; private set; }

        public static Color BorderColor { get; private set; }

        #endregion

        #region Constructors

        static CustomEditorStyles()
        {
            Heading1                = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize            = 18,
                wordWrap            = true,
                richText            = true,
                alignment           = TextAnchor.MiddleLeft,
            };
            Heading2                = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize            = 16,
                wordWrap            = true,
                richText            = true,
                alignment           = TextAnchor.MiddleLeft,
            };
            Heading3                = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize            = 14,
                wordWrap            = true,
                richText            = true,
                alignment           = TextAnchor.MiddleLeft,
            };
            Normal                  = new GUIStyle(EditorStyles.label)
            {
                fontSize            = 14,
                wordWrap            = true,
                richText            = true,
            };
            Options                 = new GUIStyle(EditorStyles.label)
            {
                fontSize            = 12,
                wordWrap            = true,
                richText            = true,
            };
            Button                  = new GUIStyle("Button")
            {
                fontSize            = 14,
            };
            InvisibleButton         = new GUIStyle("InvisibleButton");
            Link                    = new GUIStyle("LinkLabel")
            {
                fontSize            = 12,
                wordWrap            = true,
                richText            = true,
            };
            ItemBackground          = new GUIStyle("AnimItemBackground");
            GroupBackground         = new GUIStyle("HelpBox");
            GroupBackground.margin  = new RectOffset(GroupBackground.margin.left, GroupBackground.margin.right, GroupBackground.margin.top, 5);
            BorderColor             = new Color(0.15f, 0.15f, 0.15f, 1f);
        }

        #endregion
    }
}