--[[--
 * @Description: 目前只负责ui-scene绑定关系
 * @Author:      armyshu
 * @FileName:    ui_base.lua
 * @DateTime:    2016-01-13 19:39:33
 ]]

require "Logic/Scenesys/scene_type"
require "Framework/ui_stack_mgr"

ui_base = 
{
	sceneBelong = scene_type.NONE, 
	sceneIDBelong = nil,

	isVisible = nil,
	needToHideOther = false,
}

ui_base.__index = ui_base

function ui_base.New()
	local self = {}
	setmetatable(self, ui_base)
	return self
end

--[[--
 * @Description: 注册UI-scene关系，使用当前SceneType
 ]]
function ui_base:RegistUSRelation(needToHideOther)
	self.sceneBelong = GetCurrSceneType()
	self.sceneIDBelong = GetCurrSceneID()
	Notifier.regist(CmdName.SHOW_SCENE, slot(self.OnSceneChange, self))
	self.needToHideOther = needToHideOther
	ui_stack_mgr.PushToStack(self)
end

--[[--
 * @Description: 注销UI-scene关系
 ]]
function ui_base:UnRegistUSRelation()
	Notifier.remove(CmdName.SHOW_SCENE, slot(self.OnSceneChange, self))
	ui_stack_mgr.PopFromStack(self)
end

--[[--
 * @Description: 响应场景跳转
 ]]
function ui_base:OnSceneChange(paras)
	if (self.sceneIDBelong ~= nil and self.sceneIDBelong ~= paras.sceneID) then
		if (not IsNil(self.gameObject)) then
			destroy(self.gameObject)
			self.gameObject = nil
		end
	end
end
