#if UNITY_ANDROID
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins.Android;
using VoxelBusters.EssentialKit.Common.Android;
namespace VoxelBusters.EssentialKit.CloudServicesCore.Android
{
    public class NativeCloudServices : NativeAndroidJavaObjectWrapper
    {
        #region Static properties

         private static AndroidJavaClass m_nativeClass;

        #endregion

        #region Constructor

        public NativeCloudServices(NativeContext context) : base(Native.kClassName, (object)context.NativeObject)
        {
        }

        #endregion
        #region Static methods
        private static AndroidJavaClass GetClass()
        {
            if (m_nativeClass == null)
            {
                m_nativeClass = new AndroidJavaClass(Native.kClassName);
            }
            return m_nativeClass;
        }
        #endregion
        #region Public methods

        public string GetFeatureName()
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : GetFeatureName]");
#endif
            return Call<string>(Native.Method.kGetFeatureName);
        }
        public void OnSyncronizeFinished(string error)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : OnSyncronizeFinished]");
#endif
            Call(Native.Method.kOnSyncronizeFinished, new object[] { error } );
        }
        public string GetSnapshot()
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : GetSnapshot]");
#endif
            return Call<string>(Native.Method.kGetSnapshot);
        }
        public string GetString(string key)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : GetString]");
#endif
            return Call<string>(Native.Method.kGetString, key);
        }
        public long GetLong(string key)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : GetLong]");
#endif
            return Call<long>(Native.Method.kGetLong, key);
        }
        public double GetDouble(string key)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : GetDouble]");
#endif
            return Call<double>(Native.Method.kGetDouble, key);
        }
        public void SetLong(string key, long value)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetLong]");
#endif
            Call(Native.Method.kSetLong, new object[] { key, value } );
        }
        public void SetDouble(string key, double value)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetDouble]");
#endif
            Call(Native.Method.kSetDouble, new object[] { key, value } );
        }
        public bool GetBool(string key)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : GetBool]");
#endif
            return Call<bool>(Native.Method.kGetBool, key);
        }
        public void SetUserChangedListener(NativeOnUserChangedListener listener)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetUserChangedListener]");
#endif
            Call(Native.Method.kSetUserChangedListener, new object[] { listener } );
        }
        public void SetExternalDataChangedListener(NativeExternalDataChangedListener listener)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetExternalDataChangedListener]");
#endif
            Call(Native.Method.kSetExternalDataChangedListener, new object[] { listener } );
        }
        public NativeBytesWrapper GetByteArray(string key)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : GetByteArray]");
#endif
            AndroidJavaObject nativeObj = Call<AndroidJavaObject>(Native.Method.kGetByteArray, key);
            NativeBytesWrapper data  = new  NativeBytesWrapper(nativeObj);
            return data;
        }
        public void SetBool(string key, bool value)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetBool]");
#endif
            Call(Native.Method.kSetBool, new object[] { key, value } );
        }
        public void SetString(string key, string value)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetString]");
#endif
            Call(Native.Method.kSetString, new object[] { key, value } );
        }
        public void SetByteArray(string key, NativeBytesWrapper value)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetByteArray]");
#endif
            Call(Native.Method.kSetByteArray, new object[] { key, value.NativeObject } );
        }
        public void RemoveKey(string key)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : RemoveKey]");
#endif
            Call(Native.Method.kRemoveKey, new object[] { key } );
        }
        public void RemoveAllKeys()
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : RemoveAllKeys]");
#endif
            Call(Native.Method.kRemoveAllKeys);
        }
        public void SetSyncInterval(float syncInterval)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : SetSyncInterval]");
#endif
            Call(Native.Method.kSetSyncInterval, new object[] { syncInterval } );
        }
        public void Syncronize(NativeSyncronizeListener listener)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("[Class : NativeCloudServices][Method : Syncronize]");
#endif
            Call(Native.Method.kSyncronize, new object[] { listener } );
        }

        #endregion

        internal class Native
        {
            internal const string kClassName = "com.voxelbusters.android.essentialkit.features.cloudservices.CloudServices";

            internal class Method
            {
                internal const string kSetByteArray = "setByteArray";
                internal const string kGetByteArray = "getByteArray";
                internal const string kOnSyncronizeFinished = "onSyncronizeFinished";
                internal const string kSetUserChangedListener = "setUserChangedListener";
                internal const string kRemoveAllKeys = "removeAllKeys";
                internal const string kGetBool = "getBool";
                internal const string kSetBool = "setBool";
                internal const string kSetLong = "setLong";
                internal const string kGetLong = "getLong";
                internal const string kGetSnapshot = "getSnapshot";
                internal const string kSetDouble = "setDouble";
                internal const string kRemoveKey = "removeKey";
                internal const string kSetString = "setString";
                internal const string kGetDouble = "getDouble";
                internal const string kSyncronize = "syncronize";
                internal const string kGetString = "getString";
                internal const string kGetFeatureName = "getFeatureName";
                internal const string kSetSyncInterval = "setSyncInterval";
                internal const string kSetExternalDataChangedListener = "setExternalDataChangedListener";
            }

        }
    }
}
#endif