/********************************************************************
	created:	2015/12/20  15:44
	file base:	DepsData
	file ext:	cs
	author:		luke
	
	purpose:	通过Json库将依赖文件记录在DepsData对象中
*********************************************************************/

using System.Collections;
using System.Collections.Generic;

namespace Best
{
    public class DepsData
    {
        public List<DepInfo> DepInfoList;
    }

    public class DepInfo
    {
        public string BundleName;
        public int ExportType;

        public List<string> DepBundleNameList;
    }
}