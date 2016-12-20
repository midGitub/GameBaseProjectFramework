/********************************************************************
	created:	2015/12/20  15:44
	file base:	AssetBundleLoaderSync
	file ext:	cs
	author:		luke
	
	purpose:	同步AB包加载器
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.IO;

namespace Best
{
    public class AssetBundleLoaderSync : AssetBundleLoader
    {
        public override AssetBundleInfo Load()
        {
            AssetBundleInfo abi = null;
            
            if (state == LoadState.State_None || state == LoadState.State_Over)
            {
                state = LoadState.State_Ready;
                abi = this.LoadDepends();
            }
            else if (state == LoadState.State_Error)
            {
                abi = this.LoadError();
            }
            else if (state == LoadState.State_Success)
            {
                abi = this.LoadSuccess();
            }

            return abi;
        }

        AssetBundleInfo LoadDepends()
        {
            if (_DepLoaders == null)
            {
                _DepLoaders = new AssetBundleLoader[BundleData.dependencies.Length];
                for (int i = 0; i < BundleData.dependencies.Length; i++)
                {
                    AssetBundleLoader depLoader = bundleManager.CreateLoader(BundleData.dependencies[i], false, assetInMemoryType);
                    _DepLoaders[i] = depLoader;
                }
            }

            for (int i = 0; i < _DepLoaders.Length; i++)
            {
                AssetBundleLoader depLoader = _DepLoaders[i];
                //Debug.LogWarning("--BundleName Sync-->" + depLoader.BundleName);
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
                    depLoader.Load();
                }

                if (depLoader.state != LoadState.State_Error && depLoader.state != LoadState.State_Success)
                {
                    depLoader.state = LoadState.State_Error;
                    AssetBundle tempAB = loadAB(depLoader.BundleName);

                    if (tempAB != null)
                    {
                        depLoader._Bundle = tempAB;
                        depLoader.state = LoadState.State_Success;
                        depLoader.LoadSuccess();
                    }
                    else
                    {
                        depLoader.LoadError();
                    }
                }

                if ((assetInMemoryType > AssetInMemoryType.Normal) && depLoader.state == LoadState.State_Success)
                {
                    _SetImmortalAsset(depLoader, assetInMemoryType);
                }
            }

            state = LoadState.State_Loading;
            _Bundle = loadAB(BundleName);
            AssetBundleInfo abi = null;
            if (_Bundle != null)
            {
                abi = LoadSuccess();
            }
            else
            {
                abi = LoadError();
            }
            
            return abi;
        }

        /// <summary>
        /// 获取StreamingAssets目录下的ab文件（同步获取）
        /// </summary>
        /// <param name="abUrl">ab包路径</param>
        /// <returns></returns>
        private AssetBundle loadAB(string bundleName)
        {
            AssetBundle assetBundle = null;

            string abUrl = BundleConfig.Instance.GetBundleUrlForSyncLoad(bundleName);

            if (File.Exists(abUrl))
            {
                byte[] bytes = File.ReadAllBytes(abUrl);
                assetBundle = AssetBundle.CreateFromMemoryImmediate(bytes);
            }
            else
            {
                Stream stream = StreamingAssetLoad.GetFile(abUrl);
                if (stream == null)
                {
                    Debug.LogError("Error 文件不存在:" + abUrl);
                    return null;
                }
                else
                {
                    ////
                    //MemCostLog.Instance.Record(eTM.eText, true);
                    byte[] bytes = bundleManager.getABReadBytes(stream.Length); //new byte[stream.Length];
                    //MemCostLog.Instance.Record(eTM.eText, false, true);
                    stream.Read(bytes, 0, (int)stream.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        assetBundle = AssetBundle.CreateFromMemoryImmediate(bytes);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("--sync-->" + ex.ToString() + "<--aburl-->" + abUrl);
                    }
                }
                stream.Close();
            }
            
            return assetBundle;
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
                    BundleInfo.AddDependency(depLoader.BundleInfo);
                }

                _Bundle = null;
            }

            if (onComplete != null)
            {
                var handler = onComplete;
                handler(BundleInfo);
                onComplete = null;
            }

            return BundleInfo;
        }

        public override AssetBundleInfo LoadError()
        {
            this.state = LoadState.State_Error;
            this.BundleInfo = null;

            if (onComplete != null)
            {
                var handler = onComplete;
                handler(BundleInfo);
                onComplete = null;
            }

            return this.BundleInfo;
        }
    }
}