using System;
using UnityEngine;
using LuaInterface;

public class Utility_LuaHelper
{
    static public void CallParaLuaFunc(LuaFunction luaFunc)
    {
        if (luaFunc != null)
        {
            luaFunc.Call();
        }
    }

    static public void CallParaLuaFunc(LuaFunction luaFunc, uint para1)
    {
        if (luaFunc != null)
        {
            luaFunc.BeginPCall();
            luaFunc.Push(para1);
            luaFunc.PCall();
            luaFunc.EndPCall();
        }
    }

    static public void CallParaLuaFunc(LuaFunction luaFunc, bool para1)
    {
        if (luaFunc != null)
        {
            luaFunc.BeginPCall();
            luaFunc.Push(para1);
            luaFunc.PCall();
            luaFunc.EndPCall();
        }
    }

    static public void CallParaLuaFunc(LuaFunction luaFunc, bool para1, float para2)
    {
        if (luaFunc != null)
        {
            luaFunc.BeginPCall();
            luaFunc.Push(para1);
            luaFunc.Push(para2);
            luaFunc.PCall();
            luaFunc.EndPCall();
        }
    }

    static public void CallParaLuaFunc(LuaFunction luaFunc, uint para1, uint para2)
    {
        if (luaFunc != null)
        {
            luaFunc.BeginPCall();
            luaFunc.Push(para1);
            luaFunc.Push(para2);
            luaFunc.PCall();
            luaFunc.EndPCall();
        }
    }

    static public void CallParaLuaFunc(LuaFunction luaFunc, string para1, string para2, string para3)
    {
        if (luaFunc != null)
        {
            luaFunc.BeginPCall();
            luaFunc.Push(para1);
            luaFunc.Push(para2);
            luaFunc.Push(para3);
            luaFunc.PCall();
            luaFunc.EndPCall();
        }
    }

    static public void CallParaLuaFunc(LuaFunction luaFunc, string para1, string para2, string para3,string para4)
    {
        if (luaFunc != null)
        {
            luaFunc.BeginPCall();
            luaFunc.Push(para1);
            luaFunc.Push(para2);
            luaFunc.Push(para3);
            luaFunc.Push(para4);
            luaFunc.PCall();
            luaFunc.EndPCall();
        }
    }
}