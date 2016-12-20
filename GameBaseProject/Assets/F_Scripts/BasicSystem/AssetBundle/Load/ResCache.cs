using System.Collections;
using System.Collections.Generic;

public class ResCache
{
    IDictionary<string, UnityEngine.Object> cachedObjsDic = null;

    private static readonly object flag = new object();
    private static ResCache instance;
    public static ResCache Instance
    {
        get
        {
            if (instance == null)
            {
                lock(flag)
                {
                    if (instance == null)
                    {
                        instance = new ResCache();
                    }
                }
            }

            return instance;
        }
    }

    public UnityEngine.Object LoadObject(string name, bool isCache)
    {
        if (cachedObjsDic == null)
        {
            cachedObjsDic = new Dictionary<string, UnityEngine.Object>();
        }
        if (cachedObjsDic.ContainsKey(name))
        {
            return cachedObjsDic[name];
        }
        else
        {
            UnityEngine.Object obj = UnityEngine.Resources.Load<UnityEngine.Object>(name);
            if (obj != null)
            {
                if (isCache)
                {
                    cachedObjsDic.Add(name, obj);
                }
                return obj;
            }
            else
            {
                return null;
            }
        }
    }

    public void DestoryObject(string name)
    {
        if (cachedObjsDic != null && cachedObjsDic.ContainsKey(name))
        {
            cachedObjsDic[name] = null;
            cachedObjsDic.Remove(name);
        }
    }
}
