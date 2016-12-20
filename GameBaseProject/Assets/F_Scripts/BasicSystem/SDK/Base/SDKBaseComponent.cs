using UnityEngine;
using System.Collections;

public class LoginRetInfo
{
    public string Uid;
    public string PlatType;
    public string Token;
    public string Passwd;
}

public class PayRetInfo
{
    public bool ret;
}

public class SDKBaseComponent
{
    virtual public void Init() { }
    virtual public void Login() { }
    virtual public void OnLoginResult(LoginRetInfo retInfo) { }
    virtual public void Pay() { }
}
