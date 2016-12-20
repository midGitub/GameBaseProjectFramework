/********************************************************************
	created:	2015/12/20  15:44
	file base:	AssetBundleLoaderAsync
	file ext:	cs
	author:		luke
	
	purpose:	异步AB包加载器
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Best
{
    public class AssetBundleLoaderAsync : AssetBundleLoader
    {
        private int currentLoadingDepCount;

        public override AssetBundleInfo Load()
        {
            if (state == LoadState.State_None || state == LoadState.State_Over)
            {
                state = LoadState.State_Ready;
                this.LoadDepends();
            }
            else if (state == LoadState.State_Error)
            {
                this.LoadError();
            }
            else if (state == LoadState.State_Success)
            {
                this.LoadSuccess();
            }

            return null;
        }

        void LoadDepends()
        {
            if (_DepLoaders == null)
            {
                _DepLoaders = new AssetBundleLoader[BundleData.dependencies.Length];
                for (int i = 0; i < BundleData.dependencies.Length; i++)
                {
                    _DepLoaders[i] = bundleManager.CreateLoader(BundleData.dependencies[i], true, assetInMemoryType);
                }
            }

            currentLoadingDepCount = 0;
            for (int i = 0; i < _DepLoaders.Length; i++)
            {
                AssetBundleLoader depLoader = _DepLoaders[i];
                //Debug.LogWarning("--BundleName Async-->" + depLoader.BundleName);
                if (depLoader.state == LoadState.State_Success)
                {
                    if (depLoader.BundleInfo != null && depLoader.BundleInfo.bundle != null)
                    {
                        if (RefTimeAddFlag)
                        {
                            //被调用的统计
                            depLoader.AddBundleRefTimes();
                        }
                    }
                    else
                    {
                        depLoader.state = LoadState.State_None;
                    }
                }

                if (depLoader.state != LoadState.State_Error && depLoader.state != LoadState.State_Success)
                {
                    currentLoadingDepCount++;
                    depLoader.onComplete += OnDepComplete;
                    depLoader.Load();
                }

                if ((assetInMemoryType > AssetInMemoryType.Normal) && depLoader.state == LoadState.State_Success)
                {
                    _SetImmortalAsset(depLoader, assetInMemoryType);
                }
            }
            this.CheckDepComplete();
        }

        public override void LoadBundle()
        {
            string abUrl = BundleConfig.Instance.GetBundleUrlForAsyncLoad(BundleName);
            GlobalObject.Instance.StartCoroutine(LoadAsset(abUrl));
        }

        private IEnumerator LoadAsset(string abUrl)
        {
            if (state != LoadState.State_Error && state != LoadState.State_Success)
            {
                if (state == LoadState.State_Ready)
                {
                    state = LoadState.State_Loading;
                    WWW www = new WWW(abUrl);
                    yield return www;

                    if (www.error == null)
                    {
                        if (state != LoadState.State_Success && state != LoadState.State_Error)
                        {
                            _Bundle = www.assetBundle;
                            
                            LoadSuccess();
                        }
                        //bundleManager.LoadComplete(this);
                    }
                    else
                    {
                        Debug.LogWarning("--Async-Fail->" + BundleName);
                        Debug.LogError("Async加载错误：" + www.error + "<--abUrl-->" + abUrl);
                        LoadError();
                    }

                    www.Dispose();
                    www = null;
                    this.Complete();
                }
                //else if (state == LoadState.State_Loading)
                //{
                //    this.Complete();
                //}
            }
        }

        void OnDepComplete(AssetBundleInfo abi)
        {
            currentLoadingDepCount--;
            this.CheckDepComplete();
        }

        void CheckDepComplete()
        {
            if (currentLoadingDepCount == 0)
            {
                bundleManager.RequestLoadBundle(this);
            }
        }

        private void Complete()
        {
            if (onComplete != null)
            {
                var handler = onComplete;
                handler(BundleInfo);
                onComplete = null;
            }
            bundleManager.LoadComplete(this);
        }

        public override AssetBundleInfo LoadSuccess()
        {
            if (BundleInfo == null || (BundleInfo != null && BundleInfo.bundle == null))
            {
                this.state = LoadState.State_Success;
                if (BundleInfo == null)
                {
                    //重新加载的统计
                    AddBundleRefTimes();
                }

                this.BundleInfo = bundleManager.CreateBundleInfo(this, _Bundle);
                this.BundleInfo.BundleLoader = this;
                if (assetInMemoryType > this.BundleInfo.assetInMemoryType)
                {
                    this.BundleInfo.assetInMemoryType = assetInMemoryType;
                }
                this.BundleInfo.OnUnloadedEvent = OnBundleUnload;
                
                foreach (AssetBundleLoader depLoader in _DepLoaders)
                {
                    if (depLoader.BundleInfo == null)
                    {
                        string abName = depLoader.BundleName.Replace(BundleConfig.Instance.BundleSuffix, "");
                        AssetBundleLoader loader = bundleManager.allLoaderCacheDic[abName];
                        BundleInfo.AddDependency(loader.BundleInfo);
                    }
                    else
                    {
                        BundleInfo.AddDependency(depLoader.BundleInfo);
                    }
                }

                _Bundle = null;
            }
            return BundleInfo;
        }

        public override AssetBundleInfo LoadError()
        {
            this.state = LoadState.State_Error;
            this.BundleInfo = null;

            return this.BundleInfo;
        }
    }
}