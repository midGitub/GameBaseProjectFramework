/********************************************************************
	created:	2015/12/20  15:44
	file base:	BundleResourceMgr
	file ext:	cs
	author:		luke
	
	purpose:	加载对象依赖配置文件，管理加载器,用来对所有资源进行统一管理，主要是加载，缓存和释放的策略
                目前包括AssetBundle和SceneBundle
*********************************************************************/

using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Best
{
    public class BundleResourceMgr
    {
        private static BundleResourceMgr instance;
        private static readonly object lockObj = new object();
        public static BundleResourceMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new BundleResourceMgr();
                        }
                    }
                }

                return instance;
            }
        }

        private byte[] ABReadBytes = new byte[3*1024 *1024];

        private DepsData depFileInfo;

        public Dictionary<string, AssetBundleData> DepInfoDic = null;

        //public delegate void LoadAssetCompleteHandler(AssetBundleInfo info);

        //同时最大的加载数
        private const int MAX_REQUEST = 5;

        private int requestRemain = MAX_REQUEST;

        //当前申请要加载的队列
        private Queue<AssetBundleLoader> requestQueue = new Queue<AssetBundleLoader>();

        private Queue<AssetBundleLoader> requestSortQueue = new Queue<AssetBundleLoader>();

        //加载队列
        private List<AssetBundleLoader> currentLoaderList = new List<AssetBundleLoader>();

        //未完成的
        private List<AssetBundleLoader> nonCompleteLoaderList = new List<AssetBundleLoader>();

        //已加载完成的缓存列表
        public Dictionary<string, AssetBundleInfo> loadFinishedAssetBundleDic = new Dictionary<string, AssetBundleInfo>();

        //已创建的所有Loader列表(包括加载完成和未完成的)
        public Dictionary<string, AssetBundleLoader> allLoaderCacheDic = new Dictionary<string, AssetBundleLoader>();

        //记录资源对象加载的次数
        private Dictionary<string, int> assetLoadTimesDic = new Dictionary<string, int>();

        //当前是否还在加载，如果加载，则暂时不回收
        private bool isCurrentLoading;

        //初始化回调，在加载完版本依赖文件后回调
        private Action initCompleteCallback;

        private bool isInitFinished = false;
        public void Init(Action callback)
        {
            if (!isInitFinished)
            {
                initCompleteCallback = callback;

                loadDepFile();
            }
        }

        private void loadDepFile()
        {
            DirectoryInfo di = new DirectoryInfo(BundleConfig.Instance.BundlesPathForPersist);
            if (!di.Exists)
                di.Create();

            string depFileUrl = string.Format("{0}{1}", BundleConfig.Instance.BundlesPathForPersist, BundleConfig.Instance.DependFileName);

            if (File.Exists(depFileUrl))
            {
                depFileInfo = JsonMapper.ToObject<DepsData>(File.ReadAllText(depFileUrl));
            }
            else
            {
                depFileUrl = "AssetBundles/" + BundleConfig.Instance.BundlePlatformStr + "/" + BundleConfig.Instance.DependFileName;
                Stream stream = StreamingAssetLoad.GetFile(depFileUrl);
                if (stream != null)
                {
                    StreamReader st = new StreamReader(stream);
                    string readContent = st.ReadToEnd();
                    depFileInfo = JsonMapper.ToObject<DepsData>(readContent);

                    st.Close();
                    stream.Close();
                }
                else
                {
                    UnityEngine.Debug.LogError("Error 首版依赖文件不存在");
                }
            }

            if (DepInfoDic == null)
            {
                DepInfoDic = new Dictionary<string, AssetBundleData>();

                if (depFileInfo != null && depFileInfo.DepInfoList != null)
                {
                    for (int i = 0; i < depFileInfo.DepInfoList.Count; i++)
                    {
                        DepInfo info = depFileInfo.DepInfoList[i];
                        AssetBundleData bundleData = new AssetBundleData();
                        bundleData.BundleName = info.BundleName;
                        bundleData.compositeType = (AssetBundleExportType)(info.ExportType);

                        if (info.DepBundleNameList != null)
                        {
                            bundleData.dependencies = info.DepBundleNameList.ToArray();
                        }

                        if (!DepInfoDic.ContainsKey(bundleData.BundleName))
                        {
                            DepInfoDic.Add(bundleData.BundleName, bundleData);
                        }
                        else
                        {
                            Debug.LogWarning("--重复相同类型文件名-->" + bundleData.BundleName);
                        }
                    }
                }
            }

            if (initCompleteCallback != null)
            {
                initCompleteCallback();
                initCompleteCallback = null;
            }

            isInitFinished = true;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="bundleName">ab包名（不带后缀.ab）</param>
        /// <param name="handler">委托回调</param>
        public void LoadAsync(AssetBundleParams abParams, AssetBundleInfo.LoadAssetCompleteHandler handler)
        {
            string bundleName = getBundleName(abParams);
            AssetBundleLoader loader = CreateLoader(bundleName, true, abParams.assetInMemoryType);
            if (loader == null)
            {
                UnityEngine.Object obj = Resources.Load(abParams.path);
                AssetBundleInfo abi = new AssetBundleInfo();
                abi.ResourcesObj = obj;
                abi.ABParams = abParams;
                if (handler != null)
                    handler(abi);

                return;
            }

            loader.IsSort = abParams.IsSort;
            loader.ABParams = mergeParams(loader.ABParams, abParams);
            if (abParams.assetInMemoryType > loader.assetInMemoryType)
            {
                loader.assetInMemoryType = abParams.assetInMemoryType;
            }

            if (loader.IsLoadOver && loader.BundleInfo != null)
            {
                if (loader.GetChildrenLoadResult())
                {
                    if (handler != null)
                    {
                        loader.SetAssetExistMode();

                        addAssetLoadTimes(bundleName);
                        loader.BundleInfo.ABParams = loader.ABParams;
                        handler(loader.BundleInfo);
                    }
                }
                else
                {
                    loader.Reset();
                    if (handler != null)
                    {
                        loader.onComplete += handler;
                    }

                    isCurrentLoading = true;
                    if (loader.state < LoadState.State_Loading)
                        nonCompleteLoaderList.Add(loader);

                    startAsynLoad();
                }
            }
            else
            {
                if (handler != null)
                {
                    loader.onComplete += handler;
                }

                isCurrentLoading = true;
                if (loader.state < LoadState.State_Loading)
                    nonCompleteLoaderList.Add(loader);

                startAsynLoad();
            }
        }

        //同步加载资源
        public AssetBundleInfo LoadSync(AssetBundleParams abParams)
        {
            string bundleName = getBundleName(abParams);
            AssetBundleLoader loader = this.CreateLoader(bundleName, false, abParams.assetInMemoryType);
            if (loader == null)
            {
                return null;
            }

            loader.ABParams = mergeParams(loader.ABParams, abParams);

            if (abParams.assetInMemoryType > loader.assetInMemoryType)
            {
                loader.assetInMemoryType = abParams.assetInMemoryType;
            }

            if (loader.IsLoadOver && loader.BundleInfo != null)
            {
                if (loader.GetChildrenLoadResult())
                {
                    loader.SetAssetExistMode();

                    addAssetLoadTimes(bundleName);
                    loader.BundleInfo.ABParams = loader.ABParams;
                    return loader.BundleInfo;
                }
                else
                {
                    loader.Reset();
                    AssetBundleInfo abi = loader.Load();
                    return abi;
                }
            }
            else
            {
                AssetBundleInfo abi = loader.Load();
                return abi;
            }
        }

        private AssetBundleParams mergeParams(AssetBundleParams loaderParam, AssetBundleParams param)
        {
            if (loaderParam != null)
            {
                if (loaderParam.parentGoQueue != null)
                {
                    Queue<GameObject> queue1 = loaderParam.parentGoQueue;
                    if (param.parentGoQueue != null)
                    {
                        Queue<GameObject> queue2 = param.parentGoQueue;

                        Queue<GameObject> retQueue = new Queue<GameObject>();
                        int total = queue1.Count;
                        for (int i = 0; i < total; i++)
                        {
                            retQueue.Enqueue(queue1.Dequeue());
                        }
                        total = queue2.Count;
                        for (int i = 0; i < total; i++)
                        {
                            retQueue.Enqueue(queue2.Dequeue());
                        }
                        param.parentGoQueue = retQueue;
                    }
                    else
                    {
                        param.parentGoQueue = queue1;
                    }
                }

                if (loaderParam.callbackActQueue != null)
                {
                    Queue<Action<GameObject, GameObject>> queue1 = loaderParam.callbackActQueue;
                    if (param.callbackActQueue != null)
                    {
                        Queue<Action<GameObject, GameObject>> queue2 = param.callbackActQueue;

                        Queue<Action<GameObject, GameObject>> retQueue = new Queue<Action<GameObject, GameObject>>();
                        int total = queue1.Count;
                        for (int i = 0; i < total; i++)
                        {
                            retQueue.Enqueue(queue1.Dequeue());
                        }
                        total = queue2.Count;
                        for (int i = 0; i < total; i++)
                        {
                            retQueue.Enqueue(queue2.Dequeue());
                        }
                        param.callbackActQueue = retQueue;
                    }
                    else
                    {
                        param.callbackActQueue = queue1;
                    }
                }
            }

            return param;
        }

        public void addAssetLoadTimes(string bundleName)
        {
            if (!assetLoadTimesDic.ContainsKey(bundleName))
                assetLoadTimesDic.Add(bundleName, 0);

            assetLoadTimesDic[bundleName] += 1;
        }

        /// <summary>
        /// 创建AB加载器
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <param name="isAsyncLoad">是否异步加载</param>
        /// <returns>加载器</returns>
        public AssetBundleLoader CreateLoader(string bundleName, bool isAsyncLoad, AssetInMemoryType assetInMemoryType)
        {
            if (bundleName.EndsWith(BundleConfig.Instance.BundleSuffix))
            {
                bundleName = bundleName.Replace(BundleConfig.Instance.BundleSuffix, "");
            }

            AssetBundleLoader loader = null;

            if (allLoaderCacheDic.ContainsKey(bundleName))
            {
                loader = allLoaderCacheDic[bundleName];
            }
            if (loader == null)
            {
                if (isAsyncLoad)
                {
                    loader = new AssetBundleLoaderAsync();
                }
                else
                {
                    loader = new AssetBundleLoaderSync();
                }
                loader.bundleManager = this;

                AssetBundleData data = null;
                if (DepInfoDic.ContainsKey(bundleName + BundleConfig.Instance.BundleSuffix))
                {
                    data = DepInfoDic[bundleName + BundleConfig.Instance.BundleSuffix];
                }
                else
                {
                    return null;
                }
                loader.BundleData = data;
                loader.BundleName = data.BundleName;

                allLoaderCacheDic[bundleName] = loader;
            }
            if (assetInMemoryType > loader.assetInMemoryType)
            {
                loader.assetInMemoryType = assetInMemoryType;
            }
            loader.RefTimeAddFlag = true;

            return loader;
        }

        /// <summary>
        /// 开始异步加载
        /// </summary>
        private void startAsynLoad()
        {
            if (nonCompleteLoaderList.Count > 0)
            {
                List<AssetBundleLoader> loaders = new List<AssetBundleLoader>(nonCompleteLoaderList);

                nonCompleteLoaderList.Clear();

                var e = loaders.GetEnumerator();
                while (e.MoveNext())
                {
                    currentLoaderList.Add(e.Current);
                }

                e = loaders.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Load();
                }
            }
        }

        /*
        /// <summary>
        /// 开始异步加载
        /// </summary>
        private void startAsynLoad()
        {
            if (nonCompleteLoaderList.Count > 0)
            {
                List<AssetBundleLoader> loaders = null;
                List<AssetBundleLoader> sortLoaders = null;
                for (int i = 0; i < nonCompleteLoaderList.Count; i++)
                {
                    if (nonCompleteLoaderList[i].IsSort)
                    {
                        if (sortLoaders == null)
                        {
                            sortLoaders = new List<AssetBundleLoader>();
                        }
                        sortLoaders.Add(nonCompleteLoaderList[i]);
                    }
                    else
                    {
                        if (loaders == null)
                        {
                            loaders = new List<AssetBundleLoader>();
                        }
                        loaders.Add(nonCompleteLoaderList[i]);
                    }
                }
                nonCompleteLoaderList.Clear();

                var e = loaders.GetEnumerator();
                while (e.MoveNext())
                {
                    currentLoaderList.Add(e.Current);
                }

                e = loaders.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Load();
                }
            }
        }
        */

        public AssetBundleInfo GetBundleInfo(string key)
        {
            if (loadFinishedAssetBundleDic.ContainsKey(key))
                return loadFinishedAssetBundleDic[key];

            return null;
        }

        private bool isLoadingSortAB = false;

        /// <summary>
        /// 请求加载Bundle，这里统一分配加载时机，防止加载太卡
        /// </summary>
        /// <param name="loader"></param>
        public void RequestLoadBundle(AssetBundleLoader loader)
        {
            if (loader.IsSort)
            {
                if (!requestSortQueue.Contains(loader))
                {
                    requestSortQueue.Enqueue(loader);
                }
            }
            else
            {
                if (!requestQueue.Contains(loader))
                {
                    requestQueue.Enqueue(loader);
                }
            }

            if (requestRemain < 0) requestRemain = 0;
            if (requestRemain > 0)
            {
                if (isLoadingSortAB)
                {
                    if (requestQueue.Count > 0)
                    {
                        AssetBundleLoader temp = requestQueue.Dequeue();
                        this.loadBundle(temp);
                    }
                }
                else
                {
                    if (requestSortQueue.Count > 0)
                    {
                        isLoadingSortAB = true;
                        AssetBundleLoader temp = requestSortQueue.Dequeue();
                        this.loadBundle(temp);
                    }
                    else
                    {
                        AssetBundleLoader temp = requestQueue.Dequeue();
                        this.loadBundle(loader);
                    }
                }
            }
        }

        void CheckRequestList()
        {
            if (requestRemain > 0 && requestSortQueue.Count > 0)
            {
                AssetBundleLoader loader = requestSortQueue.Dequeue();
                this.loadBundle(loader);
            }

            while (requestRemain > 0 && requestQueue.Count > 0)
            {
                AssetBundleLoader loader = requestQueue.Dequeue();
                this.loadBundle(loader);
            }
        }

        private void loadBundle(AssetBundleLoader loader)
        {
            if (!(loader.state == LoadState.State_Error || loader.state == LoadState.State_Success))
            {
                loader.LoadBundle();
                requestRemain--;
            }
        }

        public void LoadComplete(AssetBundleLoader loader)
        {
            if (loader.IsSort)
            {
                isLoadingSortAB = false;
            }
            requestRemain++;
            currentLoaderList.Remove(loader);

            if (currentLoaderList.Count == 0 && nonCompleteLoaderList.Count == 0)
            {
                isCurrentLoading = false;
            }
            else
            {
                this.CheckRequestList();
            }
        }

        public AssetBundleInfo CreateBundleInfo(AssetBundleLoader loader, AssetBundle assetBundle)
        {
            AssetBundleInfo abi = new AssetBundleInfo();
            abi.bundleName = loader.BundleName;
            abi.bundle = assetBundle;
            abi.data = loader.BundleData;
            abi.ABParams = loader.ABParams;
            abi.ABID = -1;
            if (abi.bundle != null)
            {
                abi.ABID = assetBundle.GetInstanceID();
            }

            string bundleName = loader.BundleName;
            if (loader.BundleName.EndsWith(BundleConfig.Instance.BundleSuffix))
                bundleName = loader.BundleName.Replace(BundleConfig.Instance.BundleSuffix, "");

            loadFinishedAssetBundleDic[bundleName] = abi;

            return abi;
        }

        public bool UnloadAssetBundle(string bundleName)
        {
            if (assetLoadTimesDic.ContainsKey(bundleName))
            {
                assetLoadTimesDic[bundleName] -= 1;
                if (assetLoadTimesDic[bundleName] == 0)
                {
                    try
                    {
                        AssetBundleInfo abi = GetBundleInfo(bundleName);
                        if (abi != null && abi.assetInMemoryType == AssetInMemoryType.Normal)
                        {
                            abi.Dispose(AssetType.Assets);

                            if (loadFinishedAssetBundleDic.ContainsKey(bundleName))
                                loadFinishedAssetBundleDic.Remove(bundleName);
                            abi = null;
                        }
                        assetLoadTimesDic.Remove(bundleName);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UnloadSceneAssetBundle(string bundleName)
        {
            if (assetLoadTimesDic.ContainsKey(bundleName))
            {
                try
                {
                    AssetBundleInfo abi = GetBundleInfo(bundleName);
                    if (abi != null && abi.assetInMemoryType == AssetInMemoryType.Normal)
                    {
                        abi.Dispose(AssetType.Scenes);

                        if (loadFinishedAssetBundleDic.ContainsKey(bundleName))
                            loadFinishedAssetBundleDic.Remove(bundleName);
                        abi = null;
                    }
                    assetLoadTimesDic.Remove(bundleName);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool UnloadImmortalAssetBundle(string bundleName)
        {
            if (assetLoadTimesDic.ContainsKey(bundleName))
            {
                assetLoadTimesDic[bundleName] -= 1;
                if (assetLoadTimesDic[bundleName] == 0)
                {
                    try
                    {
                        AssetBundleInfo abi = GetBundleInfo(bundleName);
                        if (abi != null)
                        {
                            abi.Dispose(AssetType.Assets);

                            if (loadFinishedAssetBundleDic.ContainsKey(bundleName))
                                loadFinishedAssetBundleDic.Remove(bundleName);
                            abi = null;
                        }
                        assetLoadTimesDic.Remove(bundleName);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void UnloadAllUnusedBundles()
        {
            currentLoaderList.Clear();
            requestQueue.Clear();

            List<AssetBundleInfo> tempList = new List<AssetBundleInfo>(loadFinishedAssetBundleDic.Values);

            for (int i = 0; i < tempList.Count; i++)
            {
                if (tempList[i].assetInMemoryType < AssetInMemoryType.Resident)
                {
                    string tempBundleName = tempList[i].bundleName.Replace(BundleConfig.Instance.BundleSuffix, "");
                    assetLoadTimesDic.Remove(tempBundleName);

                    tempList[i].DisposeImmediate();
                    tempList[i] = null;
                }
            }
        }

        public void DebugBundleNum(string bundleName)
        {
            foreach (var item in assetLoadTimesDic)
            {
                if (item.Key.Contains(bundleName))
                {
                    LuaInterface.Debugger.Log("name:{0},num:{1}", item.Key, item.Value);
                }
            }
        }
/*
        public void DebugAB() 
        {
            LuaInterface.Debugger.Log("ab num:{0}", assetLoadTimesDic.Count);
            foreach (var item in assetLoadTimesDic)
            {
                LuaInterface.Debugger.Log("name:{0},num:{1}", item.Key, item.Value);
            }
        }

        public void DebugForceUnloadRes(string key) 
        {
            AssetBundleInfo abInfo = GetBundleInfo(key);
            if (abInfo!=null)
            {
                abInfo.DebugDispose();
                loadFinishedAssetBundleDic.Remove(key);
            }
        }
*/

        public void DebugAllBundle()
        {
            LuaInterface.Debugger.Log("ab num:{0}", assetLoadTimesDic.Count.ToString());
            foreach (var item in assetLoadTimesDic)
            {
                LuaInterface.Debugger.Log("name:{0},num:{1}", item.Key, item.Value);
            }
        }



        private string getBundleName(AssetBundleParams abParams)
        {
            string path = abParams.path;
            string[] fileTypeArray = abParams.type.ToString().Split('.');

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(path);
            sb.Append(".");
            sb.Append(fileTypeArray[fileTypeArray.Length - 1]);
            path = sb.ToString();
            path = path.Replace(" ", "_");

            return path.Replace("/", ".");
        }

        public byte[] getABReadBytes(long num) 
        {
            if (ABReadBytes == null || num > ABReadBytes.Length)
            {
                ABReadBytes = null;
                ABReadBytes = new byte[num];
            }
            return ABReadBytes;
        }
    }
}