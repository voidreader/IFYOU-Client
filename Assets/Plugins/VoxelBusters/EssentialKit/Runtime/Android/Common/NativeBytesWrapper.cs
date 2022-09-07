#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary.NativePlugins.Android;


namespace VoxelBusters.EssentialKit.Common.Android
{
    public class NativeBytesWrapper : NativeAndroidJavaObjectWrapper
    {
#region Fields

        private const string kClassName = "com.voxelbusters.android.essentialkit.common.BytesWrapper";
        private byte[] m_cachedBytes;

#endregion


        public NativeBytesWrapper(byte[] array) : base(kClassName,
#if UNITY_2019_1_OR_NEWER
            array.ToSBytes()
#else
            array
#endif
            )
        {
            Debug.Log("Creating from byte array : " + array);
        }

        public NativeBytesWrapper(AndroidJavaObject androidJavaObject) : base(kClassName, androidJavaObject)
        {
            Debug.Log("Creating from android native object : " + androidJavaObject.GetRawObject());
        }

        public byte[] GetBytes()
        {
            if (m_nativeObject == null)
                return default(byte[]);

            if(m_cachedBytes == null)
            {
#if UNITY_2019_1_OR_NEWER
                sbyte[] sbyteArray = Call<sbyte[]>("getBytes");
                Debug.Log("Successfully fetched get bytes...");
                if (sbyteArray != null)
                {
                    int length = sbyteArray.Length;
                    m_cachedBytes = new byte[length];
                    Buffer.BlockCopy(sbyteArray, 0, m_cachedBytes, 0, length);
                }
#else
                m_cachedBytes = Call<byte[]>("getBytes");
#endif
            }

            return m_cachedBytes;
        }
    }
}
#endif
