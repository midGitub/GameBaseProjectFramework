using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 封装弱引用的使用
/// </summary>
/// <typeparam name="T"></typeparam>
public class FWeakRef<T> where T :UnityEngine.Object 
{
    WeakReference m_instanceRef = null;

    public FWeakRef(T target)
    {
        m_instanceRef = new WeakReference(target);
    }

    public T Target
    {
        get
        {
            if (m_instanceRef.Target != null)
            {
                T ret = m_instanceRef.Target as T;
                return ret;
            }
            return null;
        }

        set
        {
            m_instanceRef.Target = value;
        }
    }


}
