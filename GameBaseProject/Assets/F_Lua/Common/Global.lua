--[[--
 * @Description:	Global
 * @Author:			farlye
 * @Path:			class_name
 * @DateTime:		2016-11-18 11:42:18
]]
require "Common/CmdName"
require "Framework/ui_base"
require "Framework/Notifier"
require "Framework/Utils"
require "Framework/Functions"
require "Framework/logicLuaObjMgr"

String 			= System.String
Screen			= UnityEngine.Screen
GameObject 		= UnityEngine.GameObject
Transform 		= UnityEngine.Transform
Space			= UnityEngine.Space
DictInt2Int		= System.Collections.Generic.DictInt2Int
DictInt2Double	= System.Collections.Generic.DictInt2Double
Camera			= UnityEngine.Camera
QualitySettings = UnityEngine.QualitySettings
AudioClip		= UnityEngine.AudioClip
MeshRenderer	= UnityEngine.MeshRenderer
PlayerPrefs = UnityEngine.PlayerPrefs

--游戏系统CS类
GameKernel 		= Framework.GameKernel