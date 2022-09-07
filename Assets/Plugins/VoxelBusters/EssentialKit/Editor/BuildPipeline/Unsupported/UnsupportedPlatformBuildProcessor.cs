using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.EssentialKit.Editor.Build
{
    public class UnsupportedPlatformBuildProcessor : IPreprocessBuildWithReport
    {
        #region Static methods

        private static bool IsBuildTargetSupported(BuildTarget buildTarget)
        {
            return ((BuildTarget.iOS == buildTarget) || (BuildTarget.tvOS == buildTarget) || (BuildTarget.tvOS == buildTarget));
        }

        #endregion

        #region IPreprocessBuildWithReport implementation

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            // check whether plugin is configured
            if (!EssentialKitSettingsEditorUtility.SettingsExists || IsBuildTargetSupported(report.summary.platform))
            {
                return;
            }

            DebugLogger.Log("[EssentialKit] Initiating pre-build task execution.");

            // execute tasks
            EssentialKitBuildUtility.CreateStrippingFile(report.summary.platform);

            DebugLogger.Log("[EssentialKit] Successfully completed pre-build task execution.");
        }

        #endregion
    }
}