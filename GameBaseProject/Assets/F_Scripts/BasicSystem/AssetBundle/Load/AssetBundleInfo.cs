/********************************************************************
	created:	2015/12/20  15:44
	file base:	AssetBundleInfo
	file ext:	cs
	author:		luke
	
	purpose:	加载过程中的资源包信息
*********************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Best
{
    public class AssetBundleInfo
    {
        public delegate void LoadAssetCompleteHandler(AssetBundleInfo info);

        public int ABID = -1;
        public delegate void OnUnloadedHandler(AssetBundleInfo abi);
        public OnUnloadedHandler OnUnloadedEvent;

        public AssetBundle bundle;

        public string bundleName;
        public AssetBundleData data;

        private HashSet<AssetBundleInfo> deps = new HashSet<AssetBundleInfo>();
        private List<string> depChildren = new List<string>();

        public Object ResourcesObj = null;

        public AssetBundleLoader BundleLoader = null;

        public AssetBundleParams ABParams = null;

        public AssetInMemoryType assetInMemoryType = AssetInMemoryType.Normal;
        
        public string GoPath { get; private set; }

        public Type GoType { get; private set; }

        private void setPathAndType()
        {
            if (ABParams != null)
            {
                GoPath = ABParams.path;
                GoType = ABParams.type;
            }
        }

        public void AddDependency(AssetBundleInfo target)
        {
            if (deps.Add(target))
            {
                if (target != null)
                {
                    target.depChildren.Add(this.bundleName);
                }
                else
                {
                    Debug.LogError("---AddDependency--->" + this.bundleName);
                }
            }
        }

        public void Release(AssetType assetType)
        {
            string tmp = bundleName;
            if (bundleName.EndsWith(BundleConfig.Instance.BundleSuffix))
            {
                tmp = bundleName.Replace(BundleConfig.Instance.BundleSuffix, "");
            }

            switch (assetType)
            {
                case AssetType.Assets:
                    BundleResourceMgr.Instance.UnloadAssetBundle(tmp);
                    break;
                case AssetType.Scenes:
                    BundleResourceMgr.Instance.UnloadSceneAssetBundle(tmp);
                    break;
                default:
                    break;
            }
            
        }

        public void DisposeImmediate()
        {
            unloadBundle(AssetType.Assets);

            deps.Clear();
            if (assetInMemoryType < AssetInMemoryType.Resident && OnUnloadedEvent != null)
            {
                OnUnloadedEvent(this);
            }
        }

        public void Dispose(AssetType assetType)
        {
            var e = deps.GetEnumerator();
            while (e.MoveNext())
            {
                AssetBundleInfo dep = e.Current;
                dep.depChildren.Remove(this.bundleName);
                dep.Release(assetType);
            }
            deps.Clear();
            if (assetInMemoryType < AssetInMemoryType.TempResident && OnUnloadedEvent != null)
            {
                OnUnloadedEvent(this);
            }

            unloadBundle(assetType);
        }

        public void DisposeSelf()
        {
            unloadBundle(AssetType.Assets);
        }

        private Object _mainObject;
        public virtual Object mainObject
        {
            get
            {
                if (_mainObject == null)
                {
                    try
                    {
                        _mainObject = bundle != null ? bundle.mainAsset : null;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("--get mainobj error-->" + this.bundleName + "<--ex-->" + ex.ToString());
                    }

                    if (bundle != null)
                    {
                        bundle.Unload(false);
                        BundleLoader.state = LoadState.State_Over;
                        BundleLoader.onComplete = null;
                    }
                    bundle = null;
                }
                if (_mainObject == null)
                {
                    _mainObject = ResourcesObj;
                }
                setPathAndType();
                return _mainObject;
            }
        }

        private void unloadBundle(AssetType assetType)
        {
            if (bundle != null)
            {
                switch (assetType)
                {
                    case AssetType.Assets:
                        bundle.Unload(true);
                        break;
                    case AssetType.Scenes:
                        bundle.Unload(false);
                        break;
                }
                bundle = null;
            }
        }
    }

    /*
    /// <summary>
    /// 资源包生命周期追踪器
    /// </summary>
    class GameObjectTracker : MonoBehaviour
    {
        public string bundleName;

        void Awake()
        {
            //该GameObject在属性面板隐藏
            //hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;

            if (bundleName != null)
            {
                AssetBundleInfo abi = BundleResourceMgr.Instance.GetBundleInfo(bundleName);
                if (abi != null)
                    abi.Retain(this);
            }
        }

        void OnDestroy()
        {
            if (bundleName != null)
            {
                AssetBundleInfo abi = BundleResourceMgr.Instance.GetBundleInfo(bundleName);
                if (abi != null)
                    abi.Release(this);
            }
        }
    }
    */
}

public enum AssetInMemoryType
{
    Normal = 0,
    TempResident = 1,
    Resident = 2
}

public class AssetBundleParams
{
    public string path;
    public Type type;
    public bool IsSort = false;
    public Queue<GameObject> parentGoQueue;
    public AssetInMemoryType assetInMemoryType = AssetInMemoryType.Normal;

    public Queue<Action<GameObject, GameObject>> callbackActQueue;


    public AssetBundleParams(string path, Type type)
    {
        this.path = path;
        this.type = type;
    }
}
