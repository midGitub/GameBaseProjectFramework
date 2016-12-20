using UnityEngine;
using System.Collections;
using System.IO;
using Framework;

/// <summary>
/// 只是初始化的时候进行了共享AB的加载, 并load了一个叫Dialog的Prefab,这需要看Shared AssetBundle是如何打包的
/// 另外提供了一个载入某AssetBundle的API,载入路径是Application.persistentDataPath,
/// 但在Debug模式下,使用的是Application.dataPath + "/StreamingAssets/" + target + "/"
/// 其中target分别是ios,iphone或者android
/// </summary>
public interface IResourceManager
{
    /// <summary>
    /// 开始
    /// </summary>
    void Start();
}
