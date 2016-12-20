/********************************************************************
	created:	2015/12/20  15:44
	file base:	AssetBundleExportType
	file ext:	cs
	author:		luke
	
	purpose:	定义了资源类型，需要被打成资源包的条件
*********************************************************************/

using System.IO;

namespace Best
{
    /// <summary>
    /// AssetBundle类型
    /// </summary>
    public enum AssetBundleExportType
    {
        Asset = 1,                      //普通素材，被根素材依赖的
        
        Root = 1 << 1,                  //根，需要单独打成ab包
        
        Standalone = 1 << 2,            //需要单独打包，这个素材是被两个或以上的素材依赖的
        
        RootAsset = Asset | Root        //既是根又是被别人依赖的素材，需要单独打成ab包
    }
    
    /// <summary>
    /// 导出AB的条件
    /// </summary>
    [System.Serializable]
    public struct ExportABTerms
    {
        /// <summary>
        /// 记录需要打包的资源的路径
        /// </summary>
        public string AssetDir;

        /// <summary>
        /// 记录需要打包的过滤条件，控制包的精细度
        /// </summary>
        public string[] Filter;

        /// <summary>
        /// 根据上述Filter的过滤的资源的依赖资源中如果符合SubalternFilter后缀条件的也需要打包
        /// </summary>
        public string[] SubalternFilterSuffixs;
    }
}
