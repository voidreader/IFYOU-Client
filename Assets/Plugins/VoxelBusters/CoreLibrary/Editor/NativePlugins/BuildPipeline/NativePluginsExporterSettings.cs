using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace VoxelBusters.CoreLibrary.Editor.NativePlugins.Build
{
    [Serializable, CreateAssetMenu(fileName = "NativePluginsExporterSettings", menuName = "VoxelBusters/NativePluginsExporterSettings", order = 0)]
	public partial class NativePluginsExporterSettings : ScriptableObject
	{
        #region Fields

        [SerializeField]
        private     NativePluginsExporterGroup  m_group;

        [SerializeField]
		private     bool			            m_isEnabled			= true;
		
        [SerializeField, FormerlySerializedAs("m_iOSSettings")]
		private	    IosPlatformProperties       m_iosProperties		= new IosPlatformProperties();

		#endregion

        #region Properties

        public NativePluginsExporterGroup Group
        {
            get { return m_group; }
            set { m_group   = value; }
        }

		public bool IsEnabled
		{
            get { return m_isEnabled; }
			set 
			{ 
				m_isEnabled = value; 
                ChangeInternalFileState(value);
			}
		}

		public IosPlatformProperties IosProperties
        {
            get { return m_iosProperties; }
            set { m_iosProperties   = value; }
		}

        #endregion

        #region Static methods

        public static NativePluginsExporterSettings[] FindAllExporters(bool includeInactive = false)
        {
            var     directory   = new DirectoryInfo(Application.dataPath);
            var     files       = directory.GetFiles("*.asset" , SearchOption.AllDirectories);
            var     assetPaths  = Array.ConvertAll(files, (item) =>
            {
                string      filePath    = item.FullName;
                return filePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
            });

            // filter assets
            var     exporters   = new List<NativePluginsExporterSettings>();
            foreach (string path in assetPaths)
            {
                var     scriptableObject        = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (scriptableObject is NativePluginsExporterSettings)
                {
                    // add to list
                    var     exporterSettings    = (NativePluginsExporterSettings)scriptableObject;
                    if (includeInactive || exporterSettings.IsEnabled)
                    {
                        exporters.Add(exporterSettings);
                    }
                }
            }

            return exporters.ToArray();
        }

        #endregion

        #region Private methods

        private void ChangeInternalFileState(bool active)
        { }

        #endregion
    }
}