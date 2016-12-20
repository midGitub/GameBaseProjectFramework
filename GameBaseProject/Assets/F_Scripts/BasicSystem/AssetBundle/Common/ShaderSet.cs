/********************************************************************
	created:	2016/08/19  15:07
	file base:	ShaderSet
	file ext:	cs
	author:		luke
	
	purpose:	将所有用到的Shader打成一个ab包
*********************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShaderSet : MonoBehaviour
{
    [Header("公共Shader")]
    public List<Shader> CommonShaderList = null;

    [Header("UI Shaders")]
    public List<Shader> UIShaderList = null;

    [Header("Effect Shaders")]
    public List<Shader> EffectShaderList = null;

    [Header("场景Shaders")]
    public List<Shader> SceneShaderList = null;

    [Header("角色Shaders")]
    public List<Shader> RoleShaderList = null;
}
