/********************************************************************
	created:	2015/06/18  15:57
	file base:	ResourceMgr
	file ext:	cs
	author:		army
	
	purpose:	提供加载资源的相应接口
*********************************************************************/

using Framework;
//using NS_DataCenter;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
//using NS_Level;

namespace Best
{
    public class ResourceMgr : IResourceMgr, IInitializeable
    {
        private BundleResourceMgr m_bundleResourceMgr = null;
        //private BundleResourceMgr m_loadingBundleResourceMgr = null;
        private SceneResourceMgr m_sceneResourceMgr = null;

        public void Initialize()
        {
            //加载依赖配置
            LoadConfig();
        }

        public void UnInitialize()
        {
            //卸载所有非永生的资源
            //UnloadNormalResources();
        }

        public void LoadConfig()
        {
            m_bundleResourceMgr = BundleResourceMgr.Instance;
            m_bundleResourceMgr.Init(delegate { Debug.Log("Depends Load Finished !"); });

            m_sceneResourceMgr = SceneResourceMgr.Instance;
        }

        public Texture2D LoadTextureSync(string path)
        {
            Object obj = LoadNormalObjSync(new AssetBundleParams(path, typeof(Texture2D)));
            if (obj != null)
            {
                return obj as Texture2D;
            }
            return null;
        }

        public Texture2D LoadImmortalTextureSync(string path)
        {
            Object obj = LoadResidentMemoryObjSync(new AssetBundleParams(path, typeof(Texture2D)));
            if (obj != null)
            {
                return obj as Texture2D;
            }
            return null;
        }

        public UIAtlas LoadAtlasSync(string path)
        {
            Object obj = LoadNormalObjSync(new AssetBundleParams(path, typeof(GameObject)));
            if (obj != null)
            {
                GameObject go = obj as GameObject;
                if (go != null)
                {
                    return go.GetComponent<UIAtlas>();
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// 同步方式加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Object LoadNormalObjSync(AssetBundleParams abParams)
        {
            Object retObj = null;
            AssetBundleInfo abi = m_bundleResourceMgr.LoadSync(abParams);
            if (abi != null)
                retObj = abi.mainObject;
            else
                retObj = Resources.Load(abParams.path, abParams.type);

            return retObj;
        }

        public Object LoadSceneResidentMemoryObjSync(AssetBundleParams abParams)
        {
            Object retObj = null;
            abParams.assetInMemoryType = AssetInMemoryType.TempResident;
            AssetBundleInfo abi = m_bundleResourceMgr.LoadSync(abParams);
            if (abi != null)
                retObj = abi.mainObject;
            else
                retObj = Resources.Load(abParams.path, abParams.type);

            return retObj;
        }

        //同步加载需常驻内存的资源
        public Object LoadResidentMemoryObjSync(AssetBundleParams abParams)
        {
            Object retObj = null;
            abParams.assetInMemoryType = AssetInMemoryType.Resident;
            AssetBundleInfo abi = m_bundleResourceMgr.LoadSync(abParams);
            if (abi != null)
                retObj = abi.mainObject;
            else
                retObj = Resources.Load(abParams.path, abParams.type);

            if (retObj == null)
            {
                Debugger.LogWarning(string.Format("该资源不存在，路径:{0}，类型:{1}", abParams.path, abParams.type));
            }

            return retObj;
        }

        public void LoadNormalObjAsync(AssetBundleParams abParams, AssetBundleInfo.LoadAssetCompleteHandler handle)
        {
            if (string.IsNullOrEmpty(abParams.path))
            {
                Debug.Log("加载路径为空");
                return;
            }
            
            m_bundleResourceMgr.LoadAsync(abParams, handle);
        }

        public void LoadSceneResidentMemoryObjAsync(AssetBundleParams abParams, AssetBundleInfo.LoadAssetCompleteHandler handle)
        {
            if (string.IsNullOrEmpty(abParams.path))
            {
                Debug.Log("加载路径为空");
                return;
            }
            abParams.assetInMemoryType = AssetInMemoryType.TempResident;
            m_bundleResourceMgr.LoadAsync(abParams, handle);
        }

        //异步加载需常驻内存的资源
        public void LoadResidentMemoryObjAsync(AssetBundleParams abParams, AssetBundleInfo.LoadAssetCompleteHandler handle)
        {
            if (string.IsNullOrEmpty(abParams.path))
            {
                Debug.Log("加载路径为空");
                return;
            }
            abParams.assetInMemoryType = AssetInMemoryType.Resident;
            m_bundleResourceMgr.LoadAsync(abParams, handle);
        }

        /// <summary>
        /// 异步方式加载Scene
        /// </summary>
        public void LoadSceneAsync(string sceneName, SceneLoadedCallback sceneLoadedCallback)
        {
            m_sceneResourceMgr.LoadSceneAsync(sceneName, sceneLoadedCallback);
        }

        public void UnloadLastSceneAsset(string sceneName)
        {
            //List<string> preLoadGOList = LevelPrefabData.GetInstance().GetPrefabs(sceneName);
            //if (preLoadGOList != null)
            //{
            //    for (int i = 0; i < preLoadGOList.Count; ++i)
            //    {
            //        string bundleName = getBundleName(preLoadGOList[i], typeof(GameObject));
            //        m_bundleResourceMgr.UnloadSceneAssetBundle(bundleName);
            //    }
            //}
        }
        
        //PreloadResult preloadSceneResult;
        //public void PreloadSceneAsset(string sceneName, ref PreloadResult preloadResult)
        //{
        //    this.preloadSceneResult = preloadResult;
            
        //    List<string> preLoadGOList = LevelPrefabData.GetInstance().GetPrefabs(sceneName);

        //    if (preLoadGOList != null && preLoadGOList.Count > 0)
        //    {
        //        this.preloadSceneResult.TotalCount += preLoadGOList.Count;
        //    }

        //    if (this.preloadSceneResult.TotalCount > 0)
        //    {
        //        //资源加载并生成相应预设
        //        if (preLoadGOList != null)
        //        {
        //            foreach (string item in preLoadGOList)
        //            {
        //                LoadNormalObjAsync(new AssetBundleParams(item, typeof(GameObject)), preloadGOCallBack);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        this.preloadSceneResult = null;
        //    }
        //}

        //private void preloadGOCallBack(AssetBundleInfo info)
        //{
        //    if (info != null)
        //    {
        //        if (info.mainObject != null)
        //        {
        //            GameObject go = GameObject.Instantiate(info.mainObject) as GameObject;
        //            go.name = info.mainObject.name;
        //            StaticBatchingUtility.Combine(go);
        //        }
        //    }

        //    this.preloadSceneResult.Index++;
        //    this.preloadSceneResult.PreloadPercent = 1.0f * this.preloadSceneResult.Index / this.preloadSceneResult.TotalCount;
        //}

        //PreloadResult preloadResult;
        //public void PreloadAsset(uint sceneId, ref PreloadResult preloadResult)
        //{
        //    this.preloadResult = preloadResult;

        //    IResBinData iResBinData = GameKernel.GetDataCenter().GetResBinData();
        //    List<string> preLoadObjList = iResBinData.GetPreloadAssetObjListInfoByID(sceneId);
            

        //    if (preLoadObjList != null && preLoadObjList.Count > 0)
        //    {
        //        this.preloadResult.TotalCount += preLoadObjList.Count;
        //    }
            
        //    if (this.preloadResult.TotalCount > 0)
        //    {
        //        //资源预加载到内存中
        //        if (preLoadObjList != null)
        //        {
        //            foreach (string item in preLoadObjList)
        //            {
        //                LoadSceneResidentMemoryObjAsync(new AssetBundleParams(item, typeof(GameObject)), preloadObjCallBack);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        this.preloadResult = null;
        //    }
        //}

        //private void preloadObjCallBack(AssetBundleInfo info)
        //{
        //    this.preloadResult.Index++;
        //    this.preloadResult.PreloadPercent = 1.0f * this.preloadResult.Index / this.preloadResult.TotalCount;
        //}
        

        private string getBundleName(string path, System.Type type)
        {
            if (!string.IsNullOrEmpty(path) && type != null)
            {
                string[] fileTypeArray = type.ToString().Split('.');

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append(path);
                sb.Append(".");
                sb.Append(fileTypeArray[fileTypeArray.Length - 1]);
                path = sb.ToString();
                path = path.Replace(" ", "_");

                return path.Replace("/", ".");
            }
            return "";
        }

        public bool UnloadResource(string path, System.Type type)
        {
            string bundleName = getBundleName(path, type);
            return m_bundleResourceMgr.UnloadAssetBundle(bundleName);
        }
        
        public bool UnloadImmortalResource(string path, System.Type type)
        {
            string bundleName = getBundleName(path, type);
            return m_bundleResourceMgr.UnloadImmortalAssetBundle(bundleName);
        }

        //清理加载资源时使用的资源文件，标注Immortal的资源暂不销毁
        public void UnloadAllNormalResources()
        {
            m_bundleResourceMgr.UnloadAllUnusedBundles();
            Resources.UnloadUnusedAssets();
        }
        
        public void DebugBundleNum(string bundleName)
        {
            m_bundleResourceMgr.DebugBundleNum(bundleName);
        }

        public void DebugAllBundle() 
        {
            m_bundleResourceMgr.DebugAllBundle();
        }
    }
}

