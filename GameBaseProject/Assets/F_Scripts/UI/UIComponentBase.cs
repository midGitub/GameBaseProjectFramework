using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Framework;

public abstract class UIComponentBase : MonoBehaviour
{
    public delegate void CreateInstanceDelegate();
    public delegate void DestroyInstanceDelegate();

    public delegate void UIPanelShow(UIComponentBase uiBase, bool bShow);
    public static event UIPanelShow OnUIPanelShow;

    public UIAnchor[] m_Anchor;
    Transform tweenTarget;
    public bool enableFadeInOut = true;
    public float duration = 0.2f;
    public float duration1 = 0.15f;
    Vector3 mScale;
    //bool mInitDone = false;

    protected static int m_nDepthStep = 20;
    protected static float m_fZStep = -200.0f;
    protected static int m_nCurMaxDepth = 1000;
    protected static float m_fCurMaxZ = 0;
    protected static GameObject m_kAlwaysToTopGo;
    protected static Dictionary<string, CreateInstanceDelegate> CreateInstanceTable = new Dictionary<string, CreateInstanceDelegate>();
    protected static Dictionary<string, DestroyInstanceDelegate> DestroyInstanceTable = new Dictionary<string, DestroyInstanceDelegate>();

    protected int mReleaseType;
    public int ReleaseType
    {
        get { return mReleaseType; }
        set { mReleaseType = value; }
    }

    //  2014.06.19 增加面板层次
    protected WL_UILayer.Layer Layer
    {
        get
        {
            return mLayer;
        }
        set
        {
            mLayer = value;
        }

    }
    private WL_UILayer.Layer mLayer;



    public WL_UILayer.Layer GetUILayer()
    {
        //  2014.06.19 添加层次信息
        WL_UILayer uiLayer = gameObject.GetComponent<WL_UILayer>();
        //Layer = uiLayer.layer;
        if (uiLayer == null)
        {
            return WL_UILayer.Layer.MultiLayer;
        }

        return uiLayer.layer;

    }

    public static void CreateInstanceByName(string name)
    {
        if (CreateInstanceTable.ContainsKey(name))
        {
            CreateInstanceTable[name]();
        }
    }

    public static void DestroyInstanceByName(string name)
    {
        if (DestroyInstanceTable.ContainsKey(name))
        {
            DestroyInstanceTable[name]();
        }
    }

    // 显示或隐藏整个块
    public void Visable(bool flag)
    {
        if (OnUIPanelShow != null)
        {
            OnUIPanelShow(this, flag);
        }

        gameObject.SetActive(flag);
        enabled = flag;
    }

    private void AdjustAlwaysToTopDlg()
    {
        if (m_kAlwaysToTopGo != null)
        {
            UIPanel panel = m_kAlwaysToTopGo.GetComponent<UIPanel>();
            if (panel)
            {
                panel.depth = m_nCurMaxDepth + m_nDepthStep;
                //Vector3 vecPos = new Vector3(0, 0, m_fCurMaxZ + m_fZStep);
                //m_kAlwaysToTopGo.transform.localPosition = vecPos;                
            }
        }
    }
    public void AlwaysToTop()
    {
        m_kAlwaysToTopGo = gameObject;
    }

}
