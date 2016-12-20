/********************************************************************
	created:	2015/12/20  15:44
	file base:	AssetBundleLoader
	file ext:	cs
	author:		luke
	
	purpose:	资源包加载器，任何一个ab包都是由它来加载的
*********************************************************************/

using System.Collections;
using System.IO;
using UnityEngine;

namespace Best
{
    public enum LoadState
    {
        State_None = 0,
        State_Ready = 1,
        State_Loading = 2,
        State_Error = 3,
        State_Success = 4,
        State_Over = 5
    }

    public enum AssetType
    {
        Assets,
        Scenes
    }

    public abstract class AssetBundleLoader
    {
        public AssetBundleInfo.LoadAssetCompleteHandler onComplete;

        public string BundleName;
        public AssetBundleData BundleData;
        public AssetBundleInfo BundleInfo;
        public BundleResourceMgr bundleManager;
        public LoadState state = LoadState.State_None;
        protected AssetBundleLoader[] _DepLoaders;
        public AssetBundle _Bundle;
        public AssetInMemoryType assetInMemoryType = AssetInMemoryType.Normal;
        
        public AssetBundleParams ABParams = null;
        
        public abstract AssetBundleInfo Load();
        
        public virtual void LoadBundle() { }
        
        public abstract AssetBundleInfo LoadSuccess();

        public abstract AssetBundleInfo LoadError();

        //引用次数增加标志
        public bool RefTimeAddFlag { get; set; }

        public bool IsSort = false;

        public bool GetChildrenLoadResult()
        {
            bool ret = true;
            if (_DepLoaders != null)
            {
                for (int i = 0; i < _DepLoaders.Length; i++)
                {
                    if (_DepLoaders[i] == null || (_DepLoaders[i] != null && _DepLoaders[i].state != LoadState.State_Success))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            else
            {
                ret = false;
            }
            RefTimeAddFlag = ret;
            
            return ret;
        }

        public void Reset()
        {
            if (BundleInfo != null)
            {
                BundleInfo.DisposeSelf();
            }
            BundleInfo = null;
            state = LoadState.State_None;
        }

        protected void _SetImmortalAsset(AssetBundleLoader depLoader, AssetInMemoryType assetIMType)
        {
            if (assetIMType > depLoader.BundleInfo.assetInMemoryType)
            {
                depLoader.BundleInfo.assetInMemoryType = assetIMType;
            }
            
            AssetBundleLoader[] depLoaders = depLoader._DepLoaders;
            if (depLoaders != null)
            {
                for (int i = 0; i < depLoaders.Length; i++)
                {
                    if (depLoaders[i] != null)
                    {
                        _SetImmortalAsset(depLoaders[i], assetIMType);
                    }
                }
            }
        }

        public bool IsLoadOver
        {
            get
            {
                return state == LoadState.State_Error || state == LoadState.State_Success || state == LoadState.State_Over;
            }
        }

        protected void OnBundleUnload(AssetBundleInfo abi)
        {
            this.BundleInfo = null;
            this.state = LoadState.State_None;

            string bundleName = abi.bundleName;
            if (abi.bundleName.EndsWith(BundleConfig.Instance.BundleSuffix))
                bundleName = abi.bundleName.Replace(BundleConfig.Instance.BundleSuffix, "");
            
            if (bundleManager.loadFinishedAssetBundleDic.ContainsKey(bundleName))
            {
                bundleManager.loadFinishedAssetBundleDic[bundleName] = null;
                bundleManager.loadFinishedAssetBundleDic.Remove(bundleName);
            }

            if (bundleManager.allLoaderCacheDic.ContainsKey(bundleName))
            {
                bundleManager.allLoaderCacheDic[bundleName] = null;
                bundleManager.allLoaderCacheDic.Remove(bundleName);
            }
        }

        //加载被调用或被创建都计数一次
        public void AddBundleRefTimes()
        {
            string bundleName = BundleName;
            if (BundleName.EndsWith(BundleConfig.Instance.BundleSuffix))
            {
                bundleName = BundleName.Replace(BundleConfig.Instance.BundleSuffix, "");
            }
            bundleManager.addAssetLoadTimes(bundleName);
        }

        public void SetAssetExistMode()
        {
            if (assetInMemoryType > AssetInMemoryType.Normal)
            {
                if (BundleInfo != null && assetInMemoryType > BundleInfo.assetInMemoryType)
                {
                    BundleInfo.assetInMemoryType = assetInMemoryType;
                }

                if (_DepLoaders == null)
                {
                    _DepLoaders = new AssetBundleLoader[BundleData.dependencies.Length];
                    for (int i = 0; i < BundleData.dependencies.Length; i++)
                    {
                        _DepLoaders[i] = bundleManager.CreateLoader(BundleData.dependencies[i], true, assetInMemoryType);
                    }
                }

                for (int i = 0; i < _DepLoaders.Length; i++)
                {
                    AssetBundleLoader depLoader = _DepLoaders[i];

                    if (depLoader.BundleInfo != null)
                    {
                        if (assetInMemoryType > depLoader.BundleInfo.assetInMemoryType)
                        {
                            depLoader.BundleInfo.assetInMemoryType = assetInMemoryType;
                        }
                    }

                    depLoader.SetAssetExistMode();
                }
            }
        }
    }
}

