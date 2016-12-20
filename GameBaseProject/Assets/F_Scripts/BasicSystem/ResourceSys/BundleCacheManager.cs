using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Best
{
    public enum LOADSTATUS
    {
        LOAD_ERROR = -1,     // 加载错误
        LOAD_SUCCESS = 0,    // 加载成功
    }

    /// <summary>
    /// ObjectCacheInfo用于对加载的Object进行管理
    /// </summary>
    public class ObjectCacheInfo
    {
        // 加载完成的回调委托
        public delegate void LoadCallBack(string url, Object obj, LOADSTATUS result);

        // 加载过程中的回调委托
        public delegate void LoadProcess(float percentage, LOADSTATUS status = LOADSTATUS.LOAD_SUCCESS);
    }
}
