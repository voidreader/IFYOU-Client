using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.NativePlugins
{
    public class NativeFeatureRuntimePackage
    {
        #region Properties

        public RuntimePlatform Platform
        {
            get;
            private set;
        }

        public string Assembly
        {
            get;
            private set;
        }

        public string Namespace
        {
            get;
            private set;
        }

        public string NativeInterfaceType
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        public NativeFeatureRuntimePackage(RuntimePlatform platform, string assembly, string ns, string nativeInterfaceType)
        {
            // set properties
            Platform            = platform;
            Assembly            = assembly;
            Namespace           = ns;
            NativeInterfaceType = string.Format("{0}.{1}", ns, nativeInterfaceType);
        }

        public NativeFeatureRuntimePackage(string assembly, string ns, string nativeInterfaceType)
            : this((RuntimePlatform)(-1), assembly, ns, nativeInterfaceType)
        { }

        #endregion
    }
}