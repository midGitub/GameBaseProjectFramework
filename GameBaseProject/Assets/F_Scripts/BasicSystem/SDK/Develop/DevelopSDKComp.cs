using UnityEngine;
using System.Collections;
using System;
using LuaInterface;
using System.Collections.Generic;

public class DevelopSDKComp : SDKBaseComponent
{
    //暂时 希望lua 和C#用一个获取key接口 
    string PREFIX_MACHINE = "PREFIX_MACHINE";
    string prefsNameId = "login_role_id";
    string prefsNamePasswd = "login_role_passwd";
    GameObject myReceiver = null;
    public override void Init()
    {
        base.Init();
        myReceiver = GameObject.Find(SDKManager.GameObjectName);
    }

    public override void Login()
    {
        base.Login();
        Debugger.Log("登录成功");
        Dictionary<string, string> dict = new Dictionary<string, string>();
        Util.GetString(prefsNameId);
        string uid = PlayerPrefs.GetString(PREFIX_MACHINE + prefsNameId);
        string plat_type = "QQ";
        string token = "i am" + uid + " from " + "QQ";
        string passwd = PlayerPrefs.GetString(PREFIX_MACHINE + prefsNamePasswd);
       
        if (string.IsNullOrEmpty(uid))
            token = "";

        dict.Add("user_id", uid);
        dict.Add("token", token);
        dict.Add("plat_type", plat_type);
        dict.Add("passwd", passwd);
        myReceiver.BroadcastMessage("OnLoginCallback", LitJson.JsonMapper.ToJson(dict));
    }

    public override void Pay()
    {
        base.Pay();
    }
}
