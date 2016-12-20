/* 数据中心系统
 * Author       : 
 * Data         : 2013-03-14
 * description  : 读取bin数据，并存储
 */


using UnityEngine;
using System.Collections.Generic;
using System;
//using NS_Player;

namespace NS_DataCenter
{
    public enum VLDataType
    {
        BIN_DATA,               // 所有bin数据
        RES_LOAD,               // 资源加载

        ITEM_DATA,              // 道具（物品）数据
        TEXT_DATA,              // 文本数据
        PLAYER_DATA ,           // 玩家自己的数据
        EQUIP2SKILL_DATA,       // 装备对技能的数值影响
        ATTR_CALC_DATA,         // 玩家身上的属性
        ANGER2SKILL_DATA,       // 怒气技能数据
        ENEMYLIST_DATA,         // 玩家敌人list数据
    }

    /// <summary>
    /// 
    /// </summary>
    abstract public class VLData
    {
        public virtual void Initialize(IDataCenter datacenter) { m_Parent = datacenter; }
        public virtual void UnInitialize() { m_Parent = null; }

        /// <summary>
        /// 数据中心Interface
        /// </summary>
        public IDataCenter m_Parent;
    }
    
    /// <summary>
    /// 数据中心
    /// </summary>
    public interface IDataCenter
    {
        /// <summary>
        /// 通过类型获取数据
        /// ROLE_DATA      IVLResLoad
        /// ROLE_DATA   
        /// MONSTER_DATA  
        /// LEVEL_DATA    
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        T GetDataType<T>();

        /// <summary>
        /// 开始加载配置文件
        /// </summary>
        void StartLoadConfigData();
    }
    
}

