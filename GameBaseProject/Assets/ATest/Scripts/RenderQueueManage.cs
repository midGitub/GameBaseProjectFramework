using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/**
 * @des:根据panel动态改变对象的渲染队列
 * @author:龙少
 * @注:直接挂在粒子效果的根对象上即可
 * @注:主要根据当前panel下最大渲染队列，动态改变此对象
 * ①.由于粒子效果默认renderQueue值为3000
 * ②.NGUI UIPanel默认值也为3000开始
 * ③.由于底层最终渲染顺序 （值从小到大）绘制drawCall先后顺序为(值从小到大)
 * ④.综合上述几点，为实现粒子效果在NGUI界面之间任意层，所以多出此脚本动态修改粒子对象的渲染队列值
 */

public class RenderQueueManage : MonoBehaviour
{
    //需要添加在panel显示之上的粒子效果
    public UIPanel panel;
    private List<Material> listMaterial;

    void Start()
    {
        listMaterial = new List<Material>();

        if (panel != null)
        {
            panel.addRenderQueueManage(this);
        }
        init();
    }

    private void init()
    {
        setCollectMaterials(gameObject);
        setSortMaterialRenderQueue(listMaterial);
        //      Debug.LogError ("<"+gameObject.name+">材质数:"+listMaterial.Count+"_排序完毕...");
        //      foreach (Material mat in listMaterial) {
        //          Debug.LogError ("<"+mat.name+">_渲染队列:"+mat.renderQueue);
        //      }
    }

    /**
     * @des:刷新内容
     * @注:动态添加内容时，可添加完毕后刷新跟root
     * @des:此接口可忽略，一般情况下不需调用
     */
    public void refreshRenderQueue()
    {
        listMaterial.Clear();
        init();
    }

    /**
      * @des:收集材质列表
      * @param:goChilid 当前镀锡
      */
    private void setCollectMaterials(GameObject goChild)
    {
        // --收集自己
        Renderer render = goChild.transform.renderer;
        if (render != null)
        {
            Material[] mat = render.materials;
            if (mat != null)
            {
                listMaterial.AddRange(mat);
            }
        }
        int childCount = goChild.transform.childCount;

        if (childCount <= 0)
        {
            return;
        }

        for (int i = 0; i < childCount; i++)
        {
            GameObject _go = goChild.transform.GetChild(i).gameObject;
            setCollectMaterials(_go);
            _go = null;
        }
    }

    /**
      *@des:排序材质球队列
      */
    private void setSortMaterialRenderQueue(List<Material> listMaterial)
    {
        if (listMaterial == null || listMaterial.Count <= 1)
        {
            return;
        }
        listMaterial.Sort(delegate(Material x, Material y)
        {
            if (x.renderQueue > y.renderQueue)
            {
                return 1;
            }
            if (x.renderQueue < y.renderQueue)
            {
                return -1;
            }
            if (x.renderQueue == y.renderQueue)
            {
                return 0;
            }
            return (x.GetInstanceID() < y.GetInstanceID()) ? -1 : 1;
        });
    }

    /**
      * @des:设置渲染队列值
      * @param:renderQueueValue 需改变值
      * @return true=成功 |false = 失败
      */
    public bool setRenderQueueValue(int renderQueueValue)
    {
        int count = 0;
        if (listMaterial != null && (count = listMaterial.Count) > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Material mat = listMaterial[i];
                if (mat != null)
                {
                    mat.renderQueue = renderQueueValue + i;
                }
                mat = null;
            }
            return true;
        }

        return false;
    }

    /**
      * @des:获取最大增量
      * @return 10;
      */
    public int getAddMaxRenderQueueValue()
    {
        if (listMaterial == null)
        {
            return 0;
        }
        return listMaterial.Count;
    }

    void OnDestroy()
    {
        if (panel != null)
        {
            panel.removeRenderQueueManage(this);
        }
        panel = null;
        listMaterial.Clear();
        listMaterial = null;
    }
}