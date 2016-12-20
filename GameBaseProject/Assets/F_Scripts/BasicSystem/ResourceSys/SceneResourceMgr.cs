/********************************************************************
	created:	2015/06/17  15:49
	file base:	SceneResourceMgr
	file ext:	cs
	author:		army
	
	purpose:	SceneResourceMgr用于场景加载的管理
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Best
{
    public class SceneResourceMgr
    {
        private static SceneResourceMgr instance;
        private static readonly object lockObj = new object();
        private SceneResourceMgr() { }
        public static SceneResourceMgr Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new SceneResourceMgr();
                        }
                    }
                }
                return instance;
            }
        }

        public SceneLoadedCallback onSceneLoaded;

        /// <summary>
        /// LoadScene，用于负责关卡的资源加载
        /// 需要注意的是，有没有可能上一个scene还没有加载完毕，就再次被调用
        /// 如果存在此情况的话，则需要扩展支持
        /// </summary>
        /// <param name="scene"></param>
        public void LoadSceneAsync(string scene, SceneLoadedCallback sceneLoadedCallback)
        {
            if (sceneLoadedCallback != null)
            {
                onSceneLoaded = null;
                onSceneLoaded = sceneLoadedCallback;
            }

            GlobalObject.Instance.StartCoroutine(loadScene(scene));
        }

        // 加载Scene的资源，根据scene name找到相应的AssetBundle
        private IEnumerator loadScene(string scene)
        {
            string fileUrl = string.Format("{0}/{1}/{2}/{3}", BundleConfig.Instance.PersistentDataPath, BundleConfig.Instance.BundlePlatformStr, "Levels", scene);

            //获取资源包在persistentDataPath目录下的url
            if (File.Exists(fileUrl))
            {
                string bundleUrl = "file:///" + fileUrl;
                WWW www = new WWW(bundleUrl);
                yield return www;

                if (www.error != null)
                {
                    Debug.LogError("--SceneResourcesMgr LoadScene-->" + www.error);
                }

                AssetBundle ab = www.assetBundle;
                www.Dispose();
                www = null;
                notifyResourceLoaded(scene);
                ab.Unload(false);
            }
            else
            {
                notifyResourceLoaded(scene);
            }
            yield return new WaitForEndOfFrame();
        }

        // 通知场景资源已Load完毕
        private void notifyResourceLoaded(string sceneName)
        {
            if (onSceneLoaded != null)
            {
                onSceneLoaded(sceneName);
                onSceneLoaded = null;
            }
        }
    }
}
