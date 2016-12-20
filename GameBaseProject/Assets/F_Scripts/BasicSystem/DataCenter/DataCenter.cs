using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;
//using NS_Player;
//using AttributeCalcSys;
//using NS_Actor;

namespace NS_DataCenter
{
    public enum eGlobleData
    {
        eNone = 0,
        eMonsterNotCollision,
        eMax,
    }

    public class DataCenter : IDataCenter, IInitializeable
    {
        /// <summary>
        /// 数据中心管理数据存储
        /// </summary>
        private Dictionary<VLDataType, object> m_DicDatas = new Dictionary<VLDataType, object>();

        /// <summary>
        /// 类型对应的数据类型
        /// </summary>
        private Dictionary<Type, VLDataType> m_DicType = new Dictionary<Type, VLDataType>();

        //全局的数据
        ulong m_globleState = 0;

        public void Initialize()
        {
            InitGlobleState();
        }


        public void UnInitialize()
        {

        }

        void InitGlobleState() 
        {
            m_globleState = 0;
        }

        public void StartLoadConfigData()
        {

        }

        public T GetDataType<T>()
        {
            T classT = default(T);
            Type type = typeof(T);
            if (m_DicType.ContainsKey(type) == true)
            {
                VLDataType dt = m_DicType[type];
                if (m_DicDatas.ContainsKey(dt) == true)
                {
                    object obj = m_DicDatas[dt];
                    classT = (T)obj;
                }
            }

            return classT;
        }
    }
}
