using UnityEngine;
using System.Collections;
using Framework;

public class GameAppInstaller : MonoBehaviour {
    public static GameObject m_ObjRoot = null;
 
    void Awake()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        
        CrateUIRootView();
        GameKernel.CreateGameKernel();
    }

    public static void CrateUIRootView()
    {
        UnityEngine.Object prefabRoot = Resources.Load("Prefab/UIRoot/UIRootMain");
        GameObject objRoot = GameObject.Instantiate(prefabRoot) as GameObject;
        objRoot.name = "UIRootMain";
        GameObject.DontDestroyOnLoad(objRoot);
        GameAppInstaller.m_ObjRoot = objRoot;
    }
}
