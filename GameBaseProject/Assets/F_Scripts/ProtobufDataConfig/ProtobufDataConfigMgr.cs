/********************************************************************
	created:	2015/05/27  10:11
	file base:	ProtobufDataConfigMgr
	file ext:	cs
	author:		army	
	purpose:	用来得到配置文件反序列化后的protobuf对象。
                不保存配置数据，只提供配置数据解释方法，
                每个业务模块负责存储自己的配置数据,示例用法如下：
                
                // 配置文件名
                string configFileName = "dataconfig_activity_center_conf";
                
                // 相应的protobuf对象
                ACTIVITY_CENTER_CONF_ARRAY configArray;
                configArray = ProtobufDataConfigMgr.ReadOneDataConfig<ACTIVITY_CENTER_CONF_ARRAY>(configFileName);
                
                // 挨个读取每行的数据，这里只打印其中的一列值
                for (int i = 0; i < configArray.items.Count; ++i)
                {
                    Debug.Log("i: " + i + ", description: " + configArray.items[i].description);
                }
*********************************************************************/

using dataconfig;
using ProtoBuf;
using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using LuaInterface;
using Best;

namespace ProtobufDataConfig
{
    public class ProtobufDataConfigMgr
    {
        private static Dictionary<string,Byte[]> ms_dataStreamDict = new Dictionary<string, Byte[]>();

        public static void Clear()
        {
            ms_dataStreamDict.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static LuaByteBuffer ReadConfigDataToLua(string fileName)
        {
            if (!ms_dataStreamDict.ContainsKey(fileName))
            {
                Byte[] bytes = ReadFormResource(fileName);
                ms_dataStreamDict.Add(fileName, bytes);
            }

            MemoryStream stream = new MemoryStream(ms_dataStreamDict[fileName]);
            return new LuaByteBuffer(stream.ToArray());
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T ReadConfigData<T>(string fileName)
        {
            if (!ms_dataStreamDict.ContainsKey(fileName))
            {
                Byte[] bytes = ReadFormResource(fileName);
                ms_dataStreamDict.Add(fileName, bytes);
            }

            Stream stream = new MemoryStream(ms_dataStreamDict[fileName]);

            T t = default(T);
            try
            {
                t = ReadConfigDataByStream<T>(stream);
            }
            catch (System.Exception ex)
            {
                Debug.Log("反序列化失败, 请检查数据和解析类的一致性：" + fileName);
            }
            return t;
        }

        /// <summary>
        /// 通过Stream反序列化Protobuf
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static T ReadConfigDataByStream<T>(Stream stream)
        {
            if (stream != null)
            {
                T t = Serializer.Deserialize<T>(stream);
                return t;
            }
            return default(T);
        }
        /// <summary>
        /// 通过文件名读取资源
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static byte[] ReadFormResource(string fileName)
        {
            byte[] streamBytes = null;
            string filePath = "ProtobufDataConfig/" + fileName;
            string fileUrl = string.Format("{0}{1}.bytes", BundleConfig.Instance.BundlesPathForPersist, filePath);
            if (!File.Exists(fileUrl))
            {
                TextAsset textAsset = Resources.Load<TextAsset>(fileUrl);
                if (textAsset != null)
                {
                    streamBytes = textAsset.bytes;
                }
            }
            else
            {
                using (FileStream fs = new FileStream(fileUrl, FileMode.Open))
                {
                    BinaryReader br = new BinaryReader(fs);
                    streamBytes = br.ReadBytes((int)fs.Length);
                }
            }
            return streamBytes;
        }
    }
}
