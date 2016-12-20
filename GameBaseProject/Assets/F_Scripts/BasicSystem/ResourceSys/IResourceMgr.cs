/********************************************************************
	created:	2015/06/18  11:58
	file base:	IResourceMgr
	file ext:	cs
	author:		army
	
	purpose:	用来对所有资源进行统一管理，主要是加载，缓存和释放的策略,目前存在3种不同运行时的资源情况：
                (1) 编辑器中运行
                (2) 真机运行，不打包（可以先不考虑）
                (3) 真机运行，打包
                目前包括AssetBundle和SceneBundle
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Best
{
    public delegate void SceneLoadedCallback(string sceneName);

    public interface IResourceMgr
    {
        void LoadConfig();

        /// <summary>
        /// 同步方式载入Atlas
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        UIAtlas LoadAtlasSync(string path);
        
        Texture2D LoadTextureSync(string path);

        Texture2D LoadImmortalTextureSync(string path);

        //同步加载普通资源
        Object LoadNormalObjSync(AssetBundleParams abParams);

        //同步加载临时常驻内存资源（一经加载，当前场景不会释放，但在切换场景时会被释放掉）
        Object LoadSceneResidentMemoryObjSync(AssetBundleParams abParams);

        //同步加载常驻内存资源
        Object LoadResidentMemoryObjSync(AssetBundleParams abParams);

        //异步加载普通资源
        void LoadNormalObjAsync(AssetBundleParams abParams, AssetBundleInfo.LoadAssetCompleteHandler handle);

        //异步加载临时常驻内存资源（一经加载，当前场景不会释放，但在切换场景时会被释放掉）
        void LoadSceneResidentMemoryObjAsync(AssetBundleParams abParams, AssetBundleInfo.LoadAssetCompleteHandler handle);

        //异步加载常驻内存资源
        void LoadResidentMemoryObjAsync(AssetBundleParams abParams, AssetBundleInfo.LoadAssetCompleteHandler handle);
        

        void LoadSceneAsync(string sceneName, SceneLoadedCallback sceneLoadedCallback);

        // 清除上一个场景的资源
        void UnloadLastSceneAsset(string sceneName);
        
        bool UnloadResource(string path, System.Type type);

        bool UnloadImmortalResource(string path, System.Type type);

        /// <summary>
        /// 清理加载资源时使用的资源文件，标注Immortal的资源暂不销毁
        /// </summary>
        void UnloadAllNormalResources();
        
        /// <summary>
        /// 调试bundle占的次数
        /// </summary>
        /// <param name="bundleName"></param>
        void DebugBundleNum(string bundleName);

        void DebugAllBundle();
    }
}