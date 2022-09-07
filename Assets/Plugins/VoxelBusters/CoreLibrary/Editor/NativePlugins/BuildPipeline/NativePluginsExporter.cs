using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.CoreLibrary.Editor.NativePlugins.Build
{
    public abstract class NativePluginsExporter : IPostprocessBuildWithReport
    {
        #region Constants

        private     const   string      kBaseExporterName       = "Base";

        #endregion

        #region Proprerties

        protected NativePluginsExporterSettings[] ActiveExporters { get; private set; }

        protected BuildReport Report { get; private set; }

        protected string ProjectPath => Report.summary.outputPath;

        #endregion

        #region Abstract methods

        protected abstract bool CanExportPlugins(BuildTarget target);

        #endregion

        #region Private methods

        private void Init(BuildReport report)
        {
            // set properties
            Report                      = report;

            // update base exporter state
            var     exporters           = NativePluginsExporterSettings.FindAllExporters(includeInactive: true);
            var     baseExporter        = Array.Find(exporters, (item) => string.Equals(item.name, kBaseExporterName));
            if (baseExporter != null)
            {
                baseExporter.IsEnabled  = true; // Array.Exists(exporters, (item) => (item != baseExporter) && item.IsEnabled);
            }

            // set properties
            var     canToggleFeatures   = NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState();
            ActiveExporters             = Array.FindAll(exporters, (item) => item.IsEnabled || !canToggleFeatures);
        }

        protected virtual void OnExportPlugins()
        { }

        #endregion

        #region IPostprocessBuildWithReport implementation

        public int callbackOrder => int.MaxValue;

        public virtual void OnPostprocessBuild(BuildReport report)
        {
            if (!CanExportPlugins(target: report.summary.platform)) return;

            Init(report);
            OnExportPlugins();
        }

        #endregion
    }
}