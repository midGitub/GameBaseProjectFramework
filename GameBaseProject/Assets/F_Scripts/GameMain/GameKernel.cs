using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Best;

namespace Framework
{
    public interface IInitializeable
    {
        void Initialize();
        void UnInitialize();
    }

    public interface IUpdateable
    {
        void Update();
    }

    public interface ILateUpdateable
    {
        void LateUpdate();
    }

    public interface IFixedUpdateable
    {
        void FixedUpdate();
    }
    /// <summary>
    /// 游戏组件驱动
    /// </summary>
    public class GameKernel : LuaClient
    {
        const string ObjNameGameKernel = "GameKernel";
        private static GameKernel _instance = null;
        ServiceContainer _serviceContainer = new ServiceContainer();

        IList<IInitializeable> _orderedInitializeableList = new List<IInitializeable>();

        List<Type> _inializeablesOrder = new List<Type>()
        {

        };

        public static void CreateGameKernel()
        {
            Shutdown();
            _instance = (new GameObject(ObjNameGameKernel)).AddComponent<GameKernel>();
            GameObject.DontDestroyOnLoad(_instance.gameObject);

            _instance.InitData();
        }

        new void Awake()
        {
            Instance = this;
        }

        public static void Shutdown()
        {
            if (_instance)
            {
                _instance.DoShutdown();
                _instance = null;
            }
        }

        void DoShutdown()
        {
            for (int i = _orderedInitializeableList.Count - 1; i >= 0; --i)
            {
                _orderedInitializeableList[i].UnInitialize();
            }

            _orderedInitializeableList = null;

            GameObject go = GameObject.Find(ObjNameGameKernel);
            if (go)
            {
                GameObject.Destroy(go);
            }
        }

        protected override void LoadLuaFiles()
        {
#if UNITY_EDITOR
            luaState.AddSearchPath(Application.dataPath + "/F_Lua");
#endif
            OnLoadFinished();
        }

        protected override void OnLoadFinished()
        {
            base.OnLoadFinished();
        }

        void InitData()
        {
            CreateServiceForInitData();
            ExecuteAllInitializeInfo();
            base.Init();
        }

        void CreateServiceForInitData()
        {
            IServiceLocator binder = _serviceContainer;

            binder.BindService<IResourceMgr>(new ResourceMgr());
        }

        void ExecuteAllInitializeInfo()
        {
            SortServiceInOrder<IInitializeable>(_inializeablesOrder, ref _orderedInitializeableList);

            for (int i = 0; i < _orderedInitializeableList.Count; ++i)
            {
                _orderedInitializeableList[i].Initialize();
            }
        }

        public static ResourceMgr GetResourceMgr()
        {
            if (_instance == null)
                return null;
            return _instance._serviceContainer.GetService<IResourceMgr>() as ResourceMgr;
        }

        void SortServiceInOrder<T>(IList<Type> orderList, ref IList<T> resList) where T : class
        {
            resList.Clear();
            foreach (var t in orderList)
            {
                T thisTypeService = _serviceContainer.GetService(t) as T;
                if (thisTypeService != null)
                {
                    resList.Add(thisTypeService);
                }
                else
                {
                    throw new Exception(String.Format("TypeError in specified service order list: {0} is not of type {1}",
                        t, typeof(T)));
                }
            }

            foreach (var s in _serviceContainer.AllServices)
            {
                T thisTypeService = s as T;
                if (thisTypeService != null && !resList.Contains(thisTypeService))
                {
                    resList.Add(thisTypeService);
                }
            }
        }
    }
}