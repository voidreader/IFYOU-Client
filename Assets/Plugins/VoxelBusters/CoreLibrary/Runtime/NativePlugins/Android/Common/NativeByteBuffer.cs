#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary.NativePlugins.Android
{
    public class NativeByteBuffer : NativeAndroidJavaObjectWrapper
    {
#region Fields

        private const string kClassName = "java.nio.ByteBuffer";
        private byte[] m_cachedBytes;

#endregion

        public NativeByteBuffer(AndroidJavaObject androidJavaObject) : base(kClassName, androidJavaObject)
        {
#if NATIVE_PLUGINS_DEBUG_ENABLED
            DebugLogger.Log("Creating from android native object : " + androidJavaObject.GetRawObject());
#endif
        }

        public static NativeByteBuffer Wrap(byte[] array)
        {
            if(array == null)
                return null;

            return Wrap(array.ToSBytes());
        }

        public static NativeByteBuffer Wrap(sbyte[] data)
        {
            if(data == null)
                return null;

            NativeByteBuffer nativeByteBuffer = new NativeByteBuffer(CreateFromStatic(kClassName, "wrap", data));
            return nativeByteBuffer;
        }

        public byte[] GetBytes()
        {
            if (m_nativeObject == null)
                return default(byte[]);

            if(m_cachedBytes == null)
            {
#if UNITY_2019_1_OR_NEWER
                sbyte[] sbyteArray = Call<sbyte[]>("array");
                DebugLogger.Log("Successfully fetched get bytes...");
                if (sbyteArray != null)
                {
                    int length = sbyteArray.Length;
                    m_cachedBytes = new byte[length];
                    Buffer.BlockCopy(sbyteArray, 0, m_cachedBytes, 0, length);
                }
#else
                m_cachedBytes = Call<byte[]>("array");
#endif
            }

            return m_cachedBytes;
        }

        public int size()
        {
            byte[] bytes = GetBytes();

            if(bytes == null)
                return 0;

            return bytes.Length;
        }
        
    }
}
#endif