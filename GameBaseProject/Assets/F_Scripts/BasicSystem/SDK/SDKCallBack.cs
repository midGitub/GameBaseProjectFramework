using UnityEngine;
using System.Collections;
using LuaInterface;
using LitJson;

public class SDKCallBack : MonoBehaviour
{
    public Callback<bool> OnInit = null;

    public Callback<LoginRetInfo> OnLogin = null;
    public LuaFunction OnLoginCallLua = null;

    public Callback<PayRetInfo> OnPay = null;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void OnLoginResolut(string ret)
    {

    }

    void OnInitCallback(string ret)
    {
        Debugger.Log(ret);
        if (OnInit != null)
        {
            OnInit(bool.Parse(ret.Trim()));
        }
    }

    void OnLoginCallback(string ret)
    {
        Debugger.Log(ret);
        JsonData jsonData = JsonMapper.ToObject(ret);
        if ((IDictionary)jsonData == null)
            Debugger.Log("登录返回格式不正确");

        LoginRetInfo retInfo = new LoginRetInfo();
        retInfo.Uid = ContainsKey(jsonData, "user_id") ? (string)jsonData["user_id"] : "";
        retInfo.Token = ContainsKey(jsonData, "token") ? (string)jsonData["token"] : "";
        retInfo.PlatType = ContainsKey(jsonData, "plat_type") ? (string)jsonData["plat_type"] : "";
        retInfo.Passwd = ContainsKey(jsonData, "passwd") ? (string)jsonData["passwd"] : "";

        if (OnLogin != null)
        {
            OnLogin(retInfo);
        }
        else if (OnLoginCallLua != null)
        {
            Utility_LuaHelper.CallParaLuaFunc(OnLoginCallLua, retInfo.Uid, retInfo.Token, retInfo.PlatType, retInfo.Passwd);
        }
    }

    void OnPayCallback(string ret)
    {
        Debugger.Log(ret);
        if (OnPay != null)
        {
            JsonData jsonData = JsonMapper.ToObject(ret);
            if (((IDictionary)jsonData) == null)
                Debugger.Log("支付返回格式不正确");

            PayRetInfo retInfo = new PayRetInfo();
            OnPay(retInfo);
        }
    }

    bool ContainsKey(JsonData jsonData, string name)
    {
        if(((IDictionary)jsonData).Contains(name))
        {
            return true;
        }
        return false;
    }
}
