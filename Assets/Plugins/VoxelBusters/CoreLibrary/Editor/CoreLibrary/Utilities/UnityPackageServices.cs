using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.Editor
{
    public static class UnityPackageServices
    {
        #region Constants

        private     static  readonly    string[]        s_staticFolders     = { "Essentials", "Examples", "Extras", "Plugins" };

        #endregion

        #region Static methods

        public static void MigrateToUPM(this UnityPackageDefinition package)
        {
            try
            {
                // confirm that package exists in default install path
                if (!package.IsInstalledWithinAssets()) return;

                // move files and folders to new path
                var     sourceDirectory = new DirectoryInfo(package.DefaultInstallPath);
                IOServices.CreateDirectory(package.UpmInstallPath);
                foreach (var file in sourceDirectory.GetFiles())
                {
                    var     fileName    = file.Name;
                    if (System.Array.Exists(s_staticFolders, (item) => string.Equals(fileName, $"{item}.meta")))
                    {
                        continue;
                    }
                    IOServices.MoveFile(file.FullName, $"{package.UpmInstallPath}/{fileName}");
                }
                foreach (var subDirectory in sourceDirectory.GetDirectories())
                {
                    var     subDirectoryName    = subDirectory.Name;
                    if (System.Array.Exists(s_staticFolders, (item) => string.Equals(subDirectoryName, item)))
                    {
                        continue;
                    }
                    IOServices.MoveDirectory(subDirectory.FullName, $"{package.UpmInstallPath}/{subDirectoryName}");
                }
            }
            finally
            {
                AssetDatabase.Refresh();
            }
        }

        #endregion
    }
}