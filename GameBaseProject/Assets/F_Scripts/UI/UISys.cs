using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using NS_DataCenter;
using Best;

public partial class UISys : MonoBehaviour, IUIComponentContainer
{
    public static UISys Instance
    {
        get { return m_View; }
    }

    // �����ͷŷ�ʽ
    public enum EnmUIDestroyType
    {
        ENMUIDT_WITHSCENE = 0,  // ���ų����ͷŶ��ͷ�
        ENMUIDT_FREEDESTROY     // ���ɷ�ʽ�����泡���ͷŶ��ͷţ��Լ�����      
    }
    partial void InitPrefabTable();

    void OnDestroy()
    {
        m_UITable.Clear();
        m_View = null;        
    }

    static UISys m_View = null;

    Dictionary<string, UIComponentBase> m_UITable = new Dictionary<string, UIComponentBase>();
    private Dictionary<string, GameObject> m_LuaUITable = new Dictionary<string, GameObject>();

    public Camera m_UICamera;
    public Transform hudRoot;
    public Transform monsterRoot;
    private int m_originalCullingMask = 32;
    public void SetUIComponent<T>(T Comp) where T : UIComponent<T>
    {
        m_UITable[typeof(T).ToString()] = Comp;
    }

    Transform[] hurtRoot = new Transform[3];
    void InitHurtRoot() 
    {
        hurtRoot[0] = this.transform.FindChild("hurtRoot/hurt_1");
        hurtRoot[1] = this.transform.FindChild("hurtRoot/hurt_2");
        hurtRoot[2] = this.transform.FindChild("hurtRoot/hurt_3");        
    }

    public Transform GetHurtTrans(int index) 
    {
        index %= 3;
        return hurtRoot[index];
    }

    void Awake()
    {
        m_View = this;
        cachedGo = gameObject;
        InitPrefabTable();
        //��¼��ԭʼֵ
        if (m_UICamera != null)
            m_originalCullingMask = m_UICamera.cullingMask;

        hudRoot = this.transform.FindChild("playerRoot");
        monsterRoot = this.transform.FindChild("monsterRoot");
        InitHurtRoot();
    }

    /// <summary>
    /// ��������������UI�������������ͣ�
    /// </summary>
    /// <param name="uiname"></param>
    public UIComponentBase CreateUIByName(string uiname)
    {
        if (m_UITable[uiname] == null)
        {
            UIComponentBase.CreateInstanceByName(uiname);
        }

        //Debug.Log("Create:" + uiname);
        if (!m_UITable[uiname].gameObject.activeInHierarchy)
        {
            SetVisableByUIName(uiname, true);
        }
        
        return m_UITable[uiname];
    }

    /// <summary>
    /// ��������lua���Ƶ����
    /// </summary>
    /// <param name="name">������֣�Ҳ��prefab��lua�ű�������</param>
    //public void CreateLuaUIPanel(string name, string luaScriptName = "")
    //{
    //    Debugger.Log("CreatePanel::>> " + name);
    //    IResourceMgr resourceMgr = GameKernel.Get<IResourceMgr>();
    //    string fullName = "Prefabs/UI/" + name;
    //    //resourceMgr.LoadResourceAsync(fullName, OnLuaPanelLoaded);
    //    UnityEngine.Object obj = resourceMgr.LoadResourceSync(fullName);
    //    OnLuaPanelLoaded(fullName, luaScriptName, obj, LOADSTATUS.LOAD_SUCCESS);
    //}

    /// <summary>
    /// �����첽������UI prefab֮��Ļص�����
    /// </summary>
    /// <param name="url"></param>
    /// <param name="obj"></param>
    /// <param name="result"></param>
    //private void OnLuaPanelLoaded(string url, string luaScriptName, UnityEngine.Object obj, LOADSTATUS result)
    //{
    //    Debug.Log("OnLuaPanelLoaded, url is: " + url);
    //    StartCreateLuaPanel(obj, url, luaScriptName);
    //}

    /// <summary>
    /// �������,�����������Լ������bundle�������Լ���Ӧlua���ƽű��Ļ���
    /// ����,����ΪpromptPanel��lua�ű�,��Ӧ��һ����promptPanel��prefab,���һᴴ��һ������Ϊ
    /// promptPanel��GameObject, Ȼ�����GameObject��addComponentһ��BaseLua�����
    /// ��������start��ʱ�����ö�Ӧ��һ��lua����:promptPanel.Start�������ͽ�
    /// lua�ǱߵĽű�����������
    /// </summary>
    //private void StartCreateLuaPanel(UnityEngine.Object resObj, string name, string luaScriptName)
    //{
    //    string tmpName = name.Substring(name.LastIndexOf('/') + 1);

    //    GameObject obj = CreateObjectByPrefab(resObj as GameObject, tmpName);
    //    BaseLua baseLua = obj.AddComponent<BaseLua>();
    //    baseLua.OnInit(null, null, luaScriptName);

    //    Debug.Log("StartCreatePanel------>>>>" + name);
    //    m_LuaUITable[tmpName] = obj;
    //}

    /// <summary>
    /// ����prefab������Object,ĿǰҲ������ulua��ʽ��UI
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private GameObject CreateObjectByPrefab(GameObject prefab, string name)
    {
        GameObject go = Instantiate(prefab) as GameObject;
        go.name = name;
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.parent = transform;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;

        return go;
    }

    /// <summary>
    /// ����ulua��ʽ��UI������ָ�����ֵ�luaPanel
    /// </summary>
    /// <param name="name"></param>
    public void DestroyLuaPanel(string name)
    {
        if (m_LuaUITable.ContainsKey(name) && null != m_LuaUITable[name])
        {
            Destroy(m_LuaUITable[name]);
        }
    }

    /// <summary>
    /// ��������������UI�������������ã�
    /// </summary>
    /// <param name="uiname"></param>
    public void DestroyUIByName(string uiname)
    {
        if (m_UITable[uiname] != null)
        {
            SetVisableByUIName(uiname,false);
            UIComponentBase.DestroyInstanceByName(uiname);
            LuaInterface.Debugger.Log("Destroy:" + uiname);
            m_UITable[uiname] = null;
        }
    }
    protected static Dictionary<WL_UILayer.Layer,  string>  mCurPanelNameList = new Dictionary<WL_UILayer.Layer,string>();
    protected static Dictionary<WL_UILayer.Layer, Queue<string>> mPanelToCreateList = new Dictionary<WL_UILayer.Layer, Queue<string>>();
    
    public void SetVisableByUIName(string uiname, bool bShow)
    { 
        UIComponentBase uiPanel = m_UITable[uiname];
        if (uiPanel != null)
        {
            WL_UILayer.Layer uiLayer = uiPanel.GetUILayer();

            // �����ֱ�Ӵ���
            if (uiLayer == WL_UILayer.Layer.MainBarLayer)
            {
                uiPanel.Visable(bShow);
                return;
            }

            // �������㱣������
            if (mCurPanelNameList.ContainsKey(uiLayer) == false)
            {
                mCurPanelNameList[uiLayer] = null;
                mPanelToCreateList[uiLayer] = new Queue<string>();
            }

            // �ȴ���ǰ���
            string uiNameOld = mCurPanelNameList[uiLayer];
            uiPanel.Visable(bShow);

            // �����ǻ����߼�
            if (bShow == true)
            {
                if (uiname != uiNameOld)
                {
                    // ��ͬ������������������������⣩
                    if (uiNameOld != null && uiNameOld.Length > 0)
                    {
                        UIComponentBase.DestroyInstanceByName(uiNameOld);
                        Queue<string> hidePanels = mPanelToCreateList[uiLayer];
                        hidePanels.Enqueue(uiNameOld);
                    }

                    // ���ĵ�ǰ��ʾ�����
                    mCurPanelNameList[uiLayer] = uiname;
                }
            }
            else
            {
                //��ͬ�������У��ҵ��ϸ���ʾ�ģ�����ʾ
                if (uiname != uiNameOld)
                {
                    uiPanel.Visable(bShow);
                    // ���Ʋ��˿ؼ��������봴��ʱ����ʱ�����ַ���  
                    Queue<string> hidePanels = mPanelToCreateList[uiLayer];
                    if (hidePanels.Contains(uiname))
                    {
                        Debug.Log("----hidePanels.Contains(uiPanel)---------");
                        Queue<string> tmp = new Queue<string>();
                        for (int i = 0; i < hidePanels.Count; ++i)
                        {
                            string tmpName = hidePanels.Dequeue();
                            if (tmpName == uiname)
                            {
                                break;
                            }
                            tmp.Enqueue(tmpName);
                        }

                        for (int i = 0; i < tmp.Count; ++i)
                        {
                            hidePanels.Enqueue(tmp.Dequeue());
                        }
                    }
                }
                else
                {
                    if (uiNameOld != null && uiNameOld.Length > 0)
                    {
                        mCurPanelNameList[uiLayer] = null;
                        Queue<string> hidePanels = mPanelToCreateList[uiLayer];
                        if (hidePanels != null && hidePanels.Count > 0)
                        {
                            mCurPanelNameList[uiLayer] = hidePanels.Dequeue();
                            CreateUIByName(uiNameOld);
                        }
                    }
                    else
                    {
                        mCurPanelNameList[uiLayer] = null;
                    }
                }

            }
        }
    }

    public void ChangeUICameraCullingMask(int newMask)
    {
        if (m_UICamera != null)
        {
            m_UICamera.cullingMask = newMask;
        }
    }

    public void RecoveryUICameraCullingMask()
    {
        ChangeUICameraCullingMask(m_originalCullingMask);
    }

    public void BlockTouch()
    {
        UICamera cameraComp = m_UICamera.GetComponent<UICamera>();
        cameraComp.useMouse = false;
        cameraComp.useTouch = false;
    }

    public void ActiveTouch()
    {
        UICamera cameraComp = m_UICamera.GetComponent<UICamera>();
        cameraComp.useMouse = true;
        cameraComp.useTouch = true;
    }

    public void EnableUICamera()
    {
        //Debug.LogWarning("EnableUICamera");
        UICamera cameraComp = m_UICamera.GetComponent<UICamera>();
        cameraComp.enabled = true;
    }

    public void DisableUICamera()
    {
        //Debug.LogWarning("DisableUICamera");
        UICamera cameraComp = m_UICamera.GetComponent<UICamera>();
        cameraComp.enabled = false;
    }

    GameObject cachedGo;
}
