using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.EssentialKit.AddressBookCore.Simulator;
using VoxelBusters.EssentialKit.BillingServicesCore.Simulator;
using VoxelBusters.EssentialKit.CloudServicesCore.Simulator;
using VoxelBusters.EssentialKit.GameServicesCore.Simulator;
using VoxelBusters.EssentialKit.NotificationServicesCore.Simulator;
using VoxelBusters.EssentialKit.MediaServicesCore.Simulator;

namespace VoxelBusters.EssentialKit.Editor
{
    [CustomEditor(typeof(EssentialKitSettings))]
    public class EssentialKitSettingsInspector : SettingsObjectInspector
    {
        #region Base class methods

        protected override UnityPackageDefinition GetOwner()
        {
            return EssentialKitSettings.Package;
        }

        protected override InspectorDrawStyle GetDrawStyle()
        {
            return InspectorDrawStyle.Group;
        }

        protected override ButtonInfo[] GetTopBarButtons()
        {
            return new ButtonInfo[]
            {
                new ButtonInfo(label: "Documentation",  onClick: ProductResources.OpenDocumentation),
                new ButtonInfo(label: "Tutorials",      onClick: ProductResources.OpenTutorials),
                new ButtonInfo(label: "Forum",          onClick: ProductResources.OpenForum),
                new ButtonInfo(label: "Discord",        onClick: ProductResources.OpenSupport),
                new ButtonInfo(label: "Write Review",   onClick: () => ProductResources.OpenAssetStorePage(true)),
            };
        }

        protected override PropertyGroupInfo[] GetPropertyGroups()
        {
            return new PropertyGroupInfo[]
            {
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_applicationSettings"),            displayName: "Application",             onDrawChildProperties: DrawApplicationSettingsControls),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_addressBookSettings"),            displayName: "Address Book",            onDrawChildProperties: DrawAddressBookSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_billingServicesSettings"),        displayName: "Billing Services",        onDrawChildProperties: DrawBillingServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_cloudServicesSettings"),          displayName: "Cloud Services",          onDrawChildProperties: DrawCloudServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_deepLinkServicesSettings"),       displayName: "Deep Link Services",      onDrawChildProperties: DrawDeepLinkServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_gameServicesSettings"),           displayName: "Game Services",           onDrawChildProperties: DrawGameServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_networkServicesSettings"),        displayName: "Network Services",        onDrawChildProperties: DrawNetworkServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_notificationServicesSettings"),   displayName: "Notification Services",   onDrawChildProperties: DrawNotificationServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_mediaServicesSettings"),          displayName: "Media Services",          onDrawChildProperties: DrawMediaServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_sharingServicesSettings"),        displayName: "Sharing Services",        onDrawChildProperties: DrawSharingServicesSettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_nativeUISettings"),               displayName: "Native UI",               onDrawChildProperties: DrawNativeUISettingsProperty),
                new PropertyGroupInfo(reference: serializedObject.FindProperty("m_webViewSettings"),                displayName: "WebView",                 onDrawChildProperties: DrawWebViewSettingsProperty),
            };
        }

        #endregion

        #region Section methods

        protected override void DrawFooter()
        {
            base.DrawFooter();

            // check whether we have any suggestions
            if (!NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
            {
                EditorGUILayout.HelpBox("Stripping unused features is not available with current build configuration. Please update stripping level to Medium/High in Player Settings for complete support.", MessageType.Warning);
                /*if (GUILayout.Button("Fix Now!"))
                {
                    NativeFeatureUnitySettingsBase.UpdateBuildConfigurationToSupportToggleFeatureUsageState();
                }*/
            }

            // provide option to add resources
            /*GUILayout.Space(5f);
            EditorLayoutUtility.Helpbox(
                title: "Essentials",
                description: "Add resources to your project that are essential for using Essential Kit plugin.",
                actionLabel: "Import Essentials",
                onClick: EssentialKitEditorUtility.ImportEssentialResources,
                style: CustomEditorStyles.GroupBackground);
            GUILayout.Space(5f);
            EditorLayoutUtility.Helpbox(
                title: "UPM Support",
                description: "You can install the package on UPM.",
                actionLabel: "Migrate To UPM",
                onClick: EssentialKitEditorUtility.MigratePackagesToUPM,
                style: CustomEditorStyles.GroupBackground);*/
        }

        #endregion

        #region Features methods

        private void DrawApplicationSettingsControls(SerializedProperty property)
        {
            DrawChildProperties(property);
        }

        private void DrawAddressBookSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kAddressBook);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                AddressBookSimulator.Reset();
            }
            GUILayout.EndVertical();
        }

        private void DrawBillingServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kBillingServices);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                BillingStoreSimulator.Reset();
                BillingServices.ClearPurchaseHistory(); //Need to have proper fix. Used to clear the serialized data when simulator is cleared. Currently its being saved in two different places.
            }
            GUILayout.EndVertical();
        }

        private void DrawCloudServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kCloudServices);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                CloudServicesSimulator.Reset();
            }
            GUILayout.EndVertical();
        }

        private void DrawDeepLinkServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kDeepLinkServices);
            }
            GUILayout.EndVertical();
        }

        private void DrawGameServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kGameServices);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                GameServicesSimulator.Reset();
            }
            GUILayout.EndVertical();
        }

        private void DrawNetworkServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kNetworkServices);
            }
            GUILayout.EndVertical();
        }

        private void DrawNotificationServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kNotificationServices);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                NotificationServicesSimulator.Reset();
            }
            GUILayout.EndVertical();
        }

        private void DrawNativeUISettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kNativeUI);
            }
            GUILayout.EndVertical();
        }

        private void DrawMediaServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kMediaServices);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                MediaServicesSimulator.Reset();
            }
            GUILayout.EndVertical();
        }

        private void DrawSharingServicesSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.KSharingServices);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                MediaServicesSimulator.Reset();
            }
            GUILayout.EndVertical();
        }

        private void DrawWebViewSettingsProperty(SerializedProperty property)
        {
            DrawChildProperties(property, ignoreProperties: "m_isEnabled");

            GUILayout.BeginVertical();
            if (GUILayout.Button("Resource Page"))
            {
                ProductResources.OpenResourcePage(NativeFeatureType.kWebView);
            }
            if (GUILayout.Button("Reset Simulator"))
            {
                MediaServicesSimulator.Reset();
            }
            GUILayout.EndVertical();
        }

        #endregion
    }
}