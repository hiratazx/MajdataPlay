using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MajdataPlay.Platform.Android.OS
{
    public class JavaObject
    {
        const string FULLY_QUALIFIED_NAME = "java.lang.Object";
        public virtual string FullyQualifiedName { get; } = FULLY_QUALIFIED_NAME;

        protected readonly static IntPtr ClassHandle = IntPtr.Zero;
        protected IntPtr ObjectHandle = IntPtr.Zero;

        static JavaObject()
        {
            var localClassHandle = AndroidJNI.FindClass(FULLY_QUALIFIED_NAME);
            if (localClassHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Failed to find Java class '{FULLY_QUALIFIED_NAME}'. JNI may not be initialized or the current thread is not attached to the JVM.");
            }
            ClassHandle = AndroidJNI.NewGlobalRef(localClassHandle);
            AndroidJNI.DeleteLocalRef(localClassHandle);
        }
        public JavaObject()
        {
            
        }
    }
}
