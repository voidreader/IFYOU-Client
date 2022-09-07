#if UNITY_IOS || UNITY_TVOS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;

namespace VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Xcode
{
    public class PBXNativePluginsExporter : NativePluginsExporter, IPostprocessBuildWithReport
    {
        #region Constants

        private const string                kPluginRelativePath     = "VoxelBusters/";

        private static readonly string[]    kIgnoreFileExtensions   = new string[]
        {
            ".meta", 
            ".pdf",
            ".DS_Store",
            ".mdown",
            ".asset",
            ".cs",
        };

        #endregion

        #region Fields

        private     List<string>        m_librarySearchPaths    = null;

        private     List<string>        m_frameworkSearchPaths  = null;

        #endregion

        #region Base class methods

        protected override bool CanExportPlugins(BuildTarget target)
        {
            return (BuildTarget.iOS == target) || (BuildTarget.tvOS == target);
        }

        protected override void OnExportPlugins()
        {
            base.OnExportPlugins();

            // set properties
            m_librarySearchPaths        = new List<string>();
            m_frameworkSearchPaths      = new List<string>();
            
            // remove old directory
            string  pluginExportPath    = Path.Combine(ProjectPath, kPluginRelativePath);
            IOServices.DeleteDirectory(pluginExportPath);

            // complete actions
            UpdateMacroDefinitions();
            UpdatePBXProject();
        }

        #endregion

        #region Static methods

        private static string GetProjectTarget(PBXProject project)
        {
#if UNITY_2019_3_OR_NEWER
            return project.GetUnityFrameworkTargetGuid();
#else
            return project.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif
        }

        #endregion

        #region Private methods

        private void UpdatePBXProject()
        {
            DebugLogger.Log("[XcodeBuildProcess] Linking native files.");

            // open project file for editing
            string  projectFilePath     = PBXProject.GetPBXProjectPath(ProjectPath);
            var     project             = new PBXProject();
            project.ReadFromFile(projectFilePath);
            string  targetGuid          = GetProjectTarget(project);

            // read exporter settings for adding native files 
            foreach (var featureExporter in ActiveExporters)
            {
                Debug.Log("Is Feature enabled : " + featureExporter.name + " " + featureExporter.IsEnabled);
                string  exporterFilePath    = Path.GetFullPath(AssetDatabase.GetAssetPath(featureExporter));
                string  exporterFolder      = Path.GetDirectoryName(exporterFilePath);
                var     iOSSettings         = featureExporter.IosProperties;
                string  exporterGroup       = GetExportGroupPath(exporterSettings: featureExporter, prefixPath: kPluginRelativePath);

                // add files
                foreach (var fileInfo in iOSSettings.Files)
                {
                    AddFileToProject(project, fileInfo.AbsoultePath, targetGuid, exporterGroup, fileInfo.CompileFlags);
                }

                // add folder
                foreach (var folderInfo in iOSSettings.Folders)
                {
                    AddFolderToProject(project, folderInfo.AbsoultePath, targetGuid, exporterGroup, folderInfo.CompileFlags);
                }

                // add headerpaths
                foreach (var pathInfo in iOSSettings.HeaderPaths)
                {
                    string  destinationPath = GetFilePathInProject(pathInfo.AbsoultePath, exporterFolder, exporterGroup);
                    string  formattedPath   = FormatFilePathInProject(destinationPath);
                    project.AddHeaderSearchPath(targetGuid, formattedPath);
                }

                // add frameworks
                foreach (var framework in iOSSettings.Frameworks)
                {
                    project.AddFrameworkToProject(targetGuid, framework.Name, framework.IsWeak);
                }
            }

            // add header search paths
            foreach (string path in m_librarySearchPaths)
            {
                project.AddLibrarySearchPath(targetGuid, FormatFilePathInProject(path));
            }

            // add framework search paths
            foreach (string path in m_frameworkSearchPaths)
            {
                project.AddFrameworkSearchPath(targetGuid, FormatFilePathInProject(path));
            }

            // apply changes
            File.WriteAllText(projectFilePath, project.WriteToString());

            // add entitlements
            AddEntitlements(project);
        }

        private string GetExportGroupPath(NativePluginsExporterSettings exporterSettings, string prefixPath)
        {
            string  groupPath               = prefixPath;
            bool    usesNestedHierarchy     = true;
            if (exporterSettings.Group != null)
            {
                groupPath                  += exporterSettings.Group.Name + "/";
                usesNestedHierarchy         = exporterSettings.Group.UsesNestedHeierarchy;
            }
            if (usesNestedHierarchy)
            {
                groupPath                  += exporterSettings.name + "/";
            }
            return groupPath;
        }

        private void AddEntitlements(PBXProject project)
        {
            // create capability manager
            string  projectFilePath     = PBXProject.GetPBXProjectPath(ProjectPath);
#if UNITY_2019_3_OR_NEWER
            var     capabilityManager   = new ProjectCapabilityManager(projectFilePath, "ios.entitlements", null, project.GetUnityMainTargetGuid());
#else
            var     capabilityManager   = new ProjectCapabilityManager(projectFilePath, "ios.entitlements", PBXProject.GetUnityTargetName());
#endif

            // add required capability
            foreach (var featureExporter in ActiveExporters)
            {
                if (!featureExporter.IsEnabled)
                    continue;

                foreach (var capability in featureExporter.IosProperties.Capabilities)
                {
                    switch (capability.Type)
                    {
                        case PBXCapabilityType.GameCenter:
                            capabilityManager.AddGameCenter();
                            break;

                        case PBXCapabilityType.iCloud:
                            capabilityManager.AddiCloud(enableKeyValueStorage: true, enableiCloudDocument: false, enablecloudKit: false, addDefaultContainers: false, customContainers: null);
                            break;

                        case PBXCapabilityType.InAppPurchase:
                            capabilityManager.AddInAppPurchase();
                            break;

                        case PBXCapabilityType.PushNotifications:
                            capabilityManager.AddPushNotifications(Debug.isDebugBuild);
                            capabilityManager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
                            break;

                        case PBXCapabilityType.AssociatedDomains:
                            var     associatedDomainsEntitlement    = capability.AssociatedDomainsEntitlement;
                            capabilityManager.AddAssociatedDomains(domains: associatedDomainsEntitlement.Domains);
                            break;

                        default:
                            throw VBException.SwitchCaseNotImplemented(capability.Type);
                    }
                }
            }

            // save changes
            capabilityManager.WriteToFile();
        }

        private void UpdateMacroDefinitions()
        {
            var     requiredMacros  = new List<string>();
            foreach (var featureExporter in ActiveExporters)
            {
                var     macros      = featureExporter.IosProperties.Macros;
                if (macros == null || macros.Length == 0)
                {
                    continue;
                }
                foreach (var entry in macros)
                {
                    if (!requiredMacros.Contains(entry))
                    {
                        requiredMacros.Add(entry);
                    }
                }
            }

            PreprocessorDirectives.WriteMacros(requiredMacros.ToArray());
        }

        private void AddFileToProject(PBXProject project, string sourceFilePath, string targetGuid, string parentGroup, string[] compileFlags)
        {
            // convert relative path to absolute path
            if (!Path.IsPathRooted(sourceFilePath))
            {
                sourceFilePath          = Path.GetFullPath(sourceFilePath);
            }

            // copy file to the target folder
            string  fileName            = Path.GetFileName(sourceFilePath);
            string  destinationFolder   = IOServices.CombinePath(ProjectPath, parentGroup);
            string  destinationFilePath = CopyFileToProject(sourceFilePath, destinationFolder);
            DebugLogger.Log(string.Format("[NativePluginsExportManager] Adding file {0} to project.", fileName));

            // add copied file to the project
            string  fileGuid            = project.AddFile(FormatFilePathInProject(destinationFilePath, rooted: false),  parentGroup + fileName);
            project.AddFileToBuildWithFlags(targetGuid, fileGuid, string.Join(" ", compileFlags));

            // add search path project
            string  fileExtension       = Path.GetExtension(destinationFilePath);
            if (string.Equals(fileExtension, ".a", StringComparison.InvariantCultureIgnoreCase))
            {
                CacheLibrarySearchPath(destinationFilePath);
            }
            else if (string.Equals(fileExtension, ".framework", StringComparison.InvariantCultureIgnoreCase))
            {
                CacheFrameworkSearchPath(destinationFilePath);
            }
        }

        private void AddFolderToProject(PBXProject project, string sourceFolder, string targetGuid, string parentGroup, string[] compileFlags)
        {
            // check whether given folder is valid
            var     sourceFolderInfo    = new DirectoryInfo(sourceFolder);
            if (!sourceFolderInfo.Exists)
            {
                return;
            }

            // add files placed within this folder
            foreach (var fileInfo in FindFiles(sourceFolderInfo))
            {
                AddFileToProject(project, fileInfo.FullName, targetGuid, parentGroup, compileFlags);
            }

            // add folders placed within this folder
            foreach (var subFolderInfo in sourceFolderInfo.GetDirectories())
            {
                string  folderGroup     = parentGroup + subFolderInfo.Name + "/";
                AddFolderToProject(project, subFolderInfo.FullName, targetGuid, folderGroup, compileFlags);
            }
        }

        private string CopyFileToProject(string filePath, string targetFolder)
        {
#if NATIVE_PLUGINS_DEBUG
            return filePath;
#else
            // create target folder directory, incase if it doesn't exist
            if (!IOServices.DirectoryExists(targetFolder))
            {
                IOServices.CreateDirectory(targetFolder);
            }

            // copy specified file
            string  fileName        = Path.GetFileName(filePath);
            string  destPath        = Path.Combine(targetFolder, fileName);

            DebugLogger.Log(string.Format("[NativePluginsExportManager] Copying file {0} to {1}.", filePath, destPath));
            FileUtil.CopyFileOrDirectory(filePath, destPath);

            return destPath;
#endif
        }

        private string GetFilePathInProject(string path, string parentFolder, string parentGroup)
        {
#if NATIVE_PLUGINS_DEBUG
            return path;
#else
            string  relativePath        = IOServices.GetRelativePath(parentFolder, path);
            string  destinationFolder   = IOServices.CombinePath(ProjectPath, parentGroup);
            return IOServices.CombinePath(destinationFolder, relativePath);
#endif
        }

        private string FormatFilePathInProject(string path, bool rooted = true)
        {
#if NATIVE_PLUGINS_DEBUG
            return path;
#else
            if (path.Contains("$(inherited)"))
            {
                return path;
            }

            string  relativePathToProject   = IOServices.GetRelativePath(ProjectPath, path);
            return rooted ? Path.Combine("$(SRCROOT)", relativePathToProject) : relativePathToProject;
#endif
        }

        private void CacheLibrarySearchPath(string path)
        {
            string  directoryPath   = Path.GetDirectoryName(path);
            if (!m_librarySearchPaths.Contains(directoryPath))
            {
                m_librarySearchPaths.Add(directoryPath);
            }
        }

        private void CacheFrameworkSearchPath(string path)
        {
            string  directoryPath   = Path.GetDirectoryName(path);
            if (!m_frameworkSearchPaths.Contains(directoryPath))
            {
                m_frameworkSearchPaths.Add(directoryPath);
            }
        }

        private FileInfo[] FindFiles(DirectoryInfo folder)
        {
            return folder.GetFiles().Where((fileInfo) =>
            {
                string  fileExtension   = fileInfo.Extension;
                return !Array.Exists(kIgnoreFileExtensions, (ignoreExt) => string.Equals(fileExtension, ignoreExt, StringComparison.InvariantCultureIgnoreCase));
            }).ToArray();
        }

        #endregion
    }
}
#endif