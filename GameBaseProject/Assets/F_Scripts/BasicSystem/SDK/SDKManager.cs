using UnityEngine;
using System.Collections;
using LuaInterface;

public enum SDKPlatform
{
    develop,
    company,
}

public class SDKManager
{
    static string gameObjectName = "SDKObject";
    public static string GameObjectName
    {
        get
        {
            return gameObjectName;
        }
    }
        
    static SDKBaseComponent mSdkImpl = null;
    static SDKCallBack mSdkCallback= null;

    static SDKBaseComponent CreateSDKComponent(SDKPlatform sdkPlatform)
    {
        switch (sdkPlatform)
        {
//#if SDK_XLCW && UNITY_ANDROID
            case SDKPlatform.company:
                return new CompanyAndroidSDKComponent();
//#elif SDK_XLCW && (UNITY_IOS || UNITY_IPHIONE)
            
//#endif
        }
        return new DevelopSDKComp();
    }
    public static void InitEnv(SDKPlatform sdkPlatform)
    {
        if (GameObject.Find(GameObjectName) == null)
        {
            GameObject obj = new GameObject(GameObjectName);
            mSdkCallback = obj.AddComponent<SDKCallBack>();
        }

        mSdkImpl = CreateSDKComponent(sdkPlatform);
    }

    #region 接口调用

    public static void InitSDK(Callback<bool> initCallback)
    {
        if (mSdkImpl != null)
        {
            mSdkCallback.OnInit = initCallback;
            mSdkImpl.Init();
        }
    }

    public static void LoginByLua(string funcName)
    {
        if (mSdkImpl != null)
        {
            mSdkCallback.OnLogin = null;
            mSdkCallback.OnLoginCallLua = LuaClient.GetMainState().GetFunction(funcName);
            mSdkImpl.Login();
        }
    }

    public static void Login(Callback<LoginRetInfo> loginCallback)
    {
        if (mSdkImpl != null)
        {
            mSdkCallback.OnLogin = mSdkImpl.OnLoginResult;
            mSdkCallback.OnLoginCallLua = null;
            mSdkImpl.Login();
        }
    }

    public static void Pay(Callback<PayRetInfo> payCallback)
    {
        if (mSdkImpl != null)
        {
            mSdkCallback.OnPay = payCallback;
            mSdkImpl.Pay();
        }
    }

    #endregion
}
